using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class ApplicationRuntime : IApplication, IDisposable
    {
        private readonly bool mCloseAfterCheck;
        private readonly SystemTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;
        private readonly VersionHelper mVersionHelper;
        private readonly DispatcherTimer mTimer;
        private readonly WmiWatcher mWmiWatcher;

        private int mFailureCount;
        private SettingsView mSettingsView;
        private AboutView mAboutview;

        public ApplicationRuntime(bool closeAfterCheck)
        {
            UiThreadHelper.InitializeWithCurrentDispatcher();

            mCloseAfterCheck = closeAfterCheck;

            mTrayIcon = new SystemTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);
            mWmiWatcher = new WmiWatcher(SearchForUpdates);
            mVersionHelper = new VersionHelper();
            mTimer = new DispatcherTimer();
        }

        public void Start()
        {
            mVersionHelper.SearchForNewVersion(_OnNewVersionAvailable);

            // wait for 10 seconds (to finish startup), then search for updates
            mTimer.Interval = TimeSpan.FromSeconds(10);
            mTimer.Tick += (e, s) => SearchForUpdates();
            mTimer.Start();

            AppSettings.Instance.OnSettingsChanged = _OnSettingsChanged;

            Application.Current.Deactivated += (s, e) => _OnApplicationDeactivated();
            Application.Current.Activated += (s, e) => _OnApplicationActivated();
        }

        public void OpenSettings()
        {
            if (mSettingsView != null)
            {
                mSettingsView.BringIntoView();
                mSettingsView.Focus();

                return;
            }

            mSettingsView = new SettingsView();
            mSettingsView.DataContext = new SettingsViewModel(mSettingsView.Close, mVersionHelper);
            mSettingsView.Closed += (s, e) => mSettingsView = null;
            mSettingsView.ShowDialog();
        }

        public void SearchForUpdates()
        {
            mTimer.Stop();
            mWmiWatcher.Stop();

            var ids = AppSettings.Instance.InstallUpdates && UacHelper.IsRunningAsAdmin()
                ? AppSettings.Instance.KbIdsToInstall
                : new string[0];

            mUpdateManager.StartSearchForUpdates(ids);
            mTrayIcon.SetupToolTipAndMenuItems(TextResources.ToolTip_Searching, TextResources.ToolTip_Searching, UpdateState.Searching);
            mTrayIcon.SetIcon(UpdateState.Searching);
        }

        public void Dispose()
        {
            mTimer.Stop();
            mWmiWatcher.Stop();
            mTrayIcon.Dispose();
        }

        public void OpenWindowsUpdateControlPanel()
        {
            Process.Start("control.exe", "/name Microsoft.WindowsUpdate");
        }

        public void OpenDownloadPage()
        {
            Process.Start("http://wun.codeplex.com/releases");
        }

        public void OpenAboutDialog()
        {
            if (mAboutview != null)
            {
                mAboutview.Activate();
                return;
            }

            mAboutview = new AboutView();
            mAboutview.DataContext = new AboutViewModel(mVersionHelper, OpenDownloadPage);
            mAboutview.Show();
        }

        private void _OnSettingsChanged()
        {
            if (mTimer.IsEnabled)
                SearchForUpdates();
        }

        private void _OnNewVersionAvailable()
        {
            if (mVersionHelper.IsNewVersionAvailable)
                mTrayIcon.SetVersionMenuItem(mVersionHelper.LatestVersion.Version);
        }

        private void _OnApplicationActivated()
        {
            if (mAboutview != null)
                mAboutview.Activate();
        }

        private void _OnApplicationDeactivated()
        {
            if (mAboutview == null)
                return;

            mAboutview.Close();
            mAboutview = null;
        }

        private void _OnSearchFinished(UpdateResult result)
        {
            var message = TextResources.ToolTip_NothingFound;
            var toolTip = TextResources.ToolTip_NothingFound;

            if (result.UpdateState == UpdateState.UpdatesAvailable)
            {
                // UpdateAvailable doesn't give any information about how many updates where installed
                // but we ignore this case and only show the information that updates are available

                toolTip = message = _GetMessage(result.AvailableUpdates);
                var msg = string.Format("{0} {1}", message, TextResources.Popup_ClickToOpen);

                _ShowPopup(TextResources.Popup_UpdatesAvailableTitle, msg, result.UpdateState);

                // start listening, whether the user has installed the updates manually
                mWmiWatcher.Start();
            }
            else if (result.UpdateState == UpdateState.UpdatesInstalled)
            {
                var msg = string.Format("{0} {1}", TextResources.Popup_UpdatesInstalledMessage, TextResources.Popup_ClickToOpen);
                _ShowPopup(TextResources.Popup_UpdatesInstalledTitle, msg, result.UpdateState);
            }
            else if (result.UpdateState == UpdateState.Failure)
            {
                message = TextResources.Menu_NoConnection;
                toolTip = TextResources.ToolTip_NoConnection;
            }

            mTrayIcon.SetupToolTipAndMenuItems(toolTip, message, result.UpdateState);
            mTrayIcon.SetIcon(result.UpdateState);

            if (mCloseAfterCheck && (result.UpdateState.CanApplicationBeClosed(mFailureCount) || AppSettings.Instance.HideIcon))
                _CloseAfterCheck(result.UpdateState);
            else
                _StartTimer(result.UpdateState);
        }

        private void _CloseAfterCheck(UpdateState state)
        {
            // wait for the popup to be shown
            var interval = state == UpdateState.UpdatesInstalled ? 20 : 1;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(interval) };
            timer.Tick += (s, e) => Application.Current.Shutdown();
            timer.Start();
        }

        private void _StartTimer(UpdateState state)
        {
            mTimer.Interval = state == UpdateState.Failure && mFailureCount < 10
                ? TimeSpan.FromSeconds(30)
                : TimeSpan.FromMinutes(AppSettings.Instance.RefreshInterval);

            mFailureCount = state == UpdateState.Failure
                ? mFailureCount + 1
                : 0;

            mTimer.Start();
        }

        private void _ShowPopup(string title, string message, UpdateState state)
        {
            if (AppSettings.Instance.DisableNotifications)
                return;

            if (AppSettings.Instance.UseMetroStyle)
            {
                var popup = new PopupView();
                popup.DataContext = new PopupViewModel(title, message, state, popup.Close, OpenWindowsUpdateControlPanel);
                popup.Show();

                if (mSettingsView != null)
                    mSettingsView.Focus();
            }
            else
            {
                mTrayIcon.ShowBallonTip(title, message, state);
            }
        }

        private string _GetMessage(int updateCount)
        {
            return updateCount > 1
                ? string.Format(TextResources.Popup_UpdatesAvailableMessage, updateCount)
                : TextResources.Popup_OneUpdateAvailableMessage;
        }
    }
}