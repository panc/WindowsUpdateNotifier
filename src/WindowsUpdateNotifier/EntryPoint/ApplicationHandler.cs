using System;
using System.Diagnostics;
using System.Windows.Threading;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public interface IApplication
    {
        void OpenWindowsUpdateControlPanel();
        void SearchForUpdates();
    }

    public class ApplicationHandler : IApplication, IDisposable
    {
        private readonly WindowsUpdateTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;
        private readonly DispatcherTimer mTimer;
        private int mFailureCount;

        public ApplicationHandler()
        {
            mTrayIcon = new WindowsUpdateTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            mTimer = new DispatcherTimer { Interval = TimeSpan.FromHours(1) };
            mTimer.Tick += (e, s) => SearchForUpdates();
            
            SearchForUpdates();
        }

        public void SearchForUpdates()
        {
            mTimer.Stop();
            mUpdateManager.StartSearchForUpdates();
            mTrayIcon.SetToolTipAndMenuItems(TextResources.ToolTip_Searching, TextResources.ToolTip_Searching, UpdateState.Searching);
        }

        private void _OnSearchFinished(int updateCount)
        {
            var message = TextResources.ToolTip_NothingFound;
            var toolTip = TextResources.ToolTip_NothingFound;
            var state = updateCount.GetUpdateState();

            if (state == UpdateState.UpdatesAvailable)
            {
                toolTip = message = _GetMessage(updateCount);

                var popup = new PopupView();
                popup.DataContext = new PopupViewModel(TextResources.Popup_Title, message, popup.Close, this);
                popup.Show();
            }
            else if (state == UpdateState.Failure)
            {
                message = TextResources.Menu_NoConnection;
                toolTip = TextResources.ToolTip_NoConnection;
            }

            mTrayIcon.SetToolTipAndMenuItems(toolTip, message, state);
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
    }
}