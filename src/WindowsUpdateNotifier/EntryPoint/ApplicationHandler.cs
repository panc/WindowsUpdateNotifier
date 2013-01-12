using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class ApplicationHandler : IApplication, IDisposable
    {
        private readonly WindowsUpdateTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;
        private readonly DispatcherTimer mTimer;
        private int mFailureCount;

        public ApplicationHandler()
        {
            AppSettings.Initialize(_CheckWithDefaultSettings());

            mTrayIcon = new WindowsUpdateTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            AppSettings.Instance.OnSettingsChanged = _OnSettingsChanged;

            mTimer = new DispatcherTimer { Interval = TimeSpan.FromHours(1) };
            mTimer.Tick += (e, s) => SearchForUpdates();
            
            _OnSettingsChanged();
        }

        private void _OnSettingsChanged()
        {
            mTimer.Interval = TimeSpan.FromMinutes(AppSettings.Instance.RefreshInterval);
            SearchForUpdates();
        }

        public void OpenSettings()
        {
            var view = new SettingsView();
            view.DataContext = new SettingsViewModel(view.Close);
            view.ShowDialog();
        }

        public void SearchForUpdates()
        {
            mTimer.Stop();
            mUpdateManager.StartSearchForUpdates();
            mTrayIcon.SetupToolTipAndMenuItems(TextResources.ToolTip_Searching, TextResources.ToolTip_Searching, UpdateState.Searching);
            mTrayIcon.SetIcon(UpdateState.Searching);
        }

        public bool NotificationsDisabled { get; set; }

        private void _OnSearchFinished(int updateCount)
        {
            var message = TextResources.ToolTip_NothingFound;
            var toolTip = TextResources.ToolTip_NothingFound;
            var state = updateCount.GetUpdateState();

            if (state == UpdateState.UpdatesAvailable)
            {
                toolTip = message = _GetMessage(updateCount);

                if (NotificationsDisabled == false)
                {
                    var popup = new PopupView();
                    popup.DataContext = new PopupViewModel(TextResources.Popup_Title, message, popup.Close, this);
                    popup.Show();
                }
            }
            else if (state == UpdateState.Failure)
            {
                message = TextResources.Menu_NoConnection;
                toolTip = TextResources.ToolTip_NoConnection;
            }

            mTrayIcon.SetupToolTipAndMenuItems(toolTip, message, state);
            mTrayIcon.SetIcon(state);
            _StartTimer(state);
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

        public void Dispose()
        {
            mTrayIcon.Dispose();
        }

        public void OpenWindowsUpdateControlPanel()
        {
            Process.Start("control.exe", "/name Microsoft.WindowsUpdate");
        }

        private bool _CheckWithDefaultSettings()
        {
            return Environment.GetCommandLineArgs().Any(x => x == "-withDefaultSettings");
        }
    }
}