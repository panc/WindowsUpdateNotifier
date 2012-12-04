using System;
using System.Diagnostics;
using WindowsUpdateNotifier.Core;
using WindowsUpdateNotifier.Core.Resources;

namespace WindowsUpdateNotifier.Desktop
{
    public interface IApplication
    {
        void OpenSettingsView();
        void OpenWindowsUpdateControlPanel();
        void SearchForUpdates();
    }

    public class ApplicationHandler : IApplication, IDisposable
    {
        private readonly WindowsUpdateTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;

        public ApplicationHandler()
        {
            mTrayIcon = new WindowsUpdateTrayIcon(this);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            SearchForUpdates();
        }

        public void SearchForUpdates()
        {
            mUpdateManager.StartSearchForUpdates();
            mTrayIcon.SetToolTipAndMenuItems(TextResources.ToolTip_Searching, false);
        }

        private void _OnSearchFinished(int updateCount)
        {
            var message = TextResources.ToolTip_NothingFound;

            if (updateCount > 0)
            {
                message = _GetMessage(updateCount);

                var popup = new PopupView();
                popup.DataContext = new PopupViewModel(TextResources.Popup_Title, message, popup.Close, this);
                popup.Show();

                // todo
                // change icon...
            }

            mTrayIcon.SetToolTipAndMenuItems(message, true);
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

        public void OpenSettingsView()
        {
            //throw new NotImplementedException();
        }

        public void OpenWindowsUpdateControlPanel()
        {
            Process.Start("control.exe", "/name Microsoft.WindowsUpdate");
        }
    }
}