using System;
using System.Diagnostics;
using System.Windows.Threading;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class ApplicationHandler : IApplication, IDisposable
    {
        private readonly bool mCloseAfterCheck;
        private readonly WindowsUpdateTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;
        private readonly DispatcherTimer mTimer;
        private int mFailureCount;
        private SettingsView mSettingsView;

        public ApplicationHandler(bool closeAfterCheck)
        {
            mCloseAfterCheck = closeAfterCheck;

            mTrayIcon = new WindowsUpdateTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            AppSettings.Instance.OnSettingsChanged = _OnSettingsChanged;

            mTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(AppSettings.Instance.RefreshInterval) };
            mTimer.Tick += (e, s) => SearchForUpdates();

            SearchForUpdates();
        }

        public bool NotificationsDisabled { get; set; }

        public void OpenSettings()
        {
            if (mSettingsView != null)
            {
                mSettingsView.BringIntoView();
                mSettingsView.Focus();

                return;
            }

            mSettingsView = new SettingsView();
            mSettingsView.DataContext = new SettingsViewModel(mSettingsView.Close);
            mSettingsView.Closed += (s, e) => mSettingsView = null;
            mSettingsView.ShowDialog();
        }

        public void SearchForUpdates()
        {
            var ids = AppSettings.Instance.InstallUpdates && UacHelper.IsRunningAsAdmin()
                ? AppSettings.Instance.KbIdsToInstall 
                : new string[0];

            mTimer.Stop();
            mUpdateManager.StartSearchForUpdates(ids);
            mTrayIcon.SetupToolTipAndMenuItems(TextResources.ToolTip_Searching, TextResources.ToolTip_Searching, UpdateState.Searching);
            mTrayIcon.SetIcon(UpdateState.Searching);
        }

        public void Dispose()
        {
            mTimer.Stop();
            mTrayIcon.Dispose();
        }

        public void OpenWindowsUpdateControlPanel()
        {
            Process.Start("control.exe", "/name Microsoft.WindowsUpdate");
        }

        private void _OnSettingsChanged()
        {
            mTimer.Interval = TimeSpan.FromMinutes(AppSettings.Instance.RefreshInterval);
            SearchForUpdates();
        }

        private void _OnSearchFinished(UpdateResult result)
        {
            var message = TextResources.ToolTip_NothingFound;
            var toolTip = TextResources.ToolTip_NothingFound;

            if (result.UpdateState == UpdateState.UpdatesAvailable)
            {
                // UpdateAvailable doesn't give any information about how many updates where installed
                // but we ignore this case an only show the information that update are available
                
                toolTip = message = _GetMessage(result.AvailableUpdates);
                var msg = string.Format("{0} {1}", message, TextResources.Popup_ClickToOpen);

                _ShowPopup(TextResources.Popup_UpdatesAvailableTitle, msg, result.UpdateState);
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

            if (mCloseAfterCheck)
                _CloseAfterCheck(result.UpdateState);
            else
                _StartTimer(result.UpdateState);
        }

        private void _CloseAfterCheck(UpdateState state)
        {
            // wait for the popup to be shown
            var interval = state == UpdateState.UpdatesAvailable ? 20 : 1;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(interval) };
            timer.Tick += (s, e) => System.Windows.Application.Current.Shutdown();
            timer.Start();
        }

        private void _ShowPopup(string title, string message, UpdateState state)
        {
            if (NotificationsDisabled)
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

        private void _StartTimer(UpdateState state)
        {
            mTimer.Interval = state == UpdateState.Failure && mFailureCount < 10
                ? TimeSpan.FromSeconds(30)
                : TimeSpan.FromHours(1);

            mFailureCount = state == UpdateState.Failure
                ? mFailureCount + 1
                : 0;

            mTimer.Start();
        }

        private string _GetMessage(int updateCount)
        {
            return updateCount > 1
                ? string.Format(TextResources.Popup_UpdatesAvailableMessage, updateCount)
                : TextResources.Popup_OneUpdateAvailableMessage;
        }
    }
}