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

        public ApplicationHandler()
        {
            mTrayIcon = new WindowsUpdateTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            //SearchForUpdates();
            _OnSearchFinished(3);

            mTimer = new DispatcherTimer { Interval = TimeSpan.FromHours(1) };
            mTimer.Tick += (e, s) => SearchForUpdates();
            mTimer.Start();
        }

        public void SearchForUpdates()
        {
            mUpdateManager.StartSearchForUpdates();
            mTrayIcon.SetToolTipAndMenuItems(TextResources.ToolTip_Searching, false, false);
        }

        private void _OnSearchFinished(int updateCount)
        {
            var message = TextResources.ToolTip_NothingFound;
            var updatesAvailable = updateCount > 0;

            if (updatesAvailable)
            {
                message = _GetMessage(updateCount);

                var popup = new PopupView();
                popup.DataContext = new PopupViewModel(TextResources.Popup_Title, message, popup.Close, this);
                popup.Show();
            }

            mTrayIcon.SetToolTipAndMenuItems(message, true, updatesAvailable);
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