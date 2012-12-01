using System;
using WindowsUpdateNotifier.Core;
using WindowsUpdateNotifier.Core.Resources;

namespace WindowsUpdateNotifier.Desktop
{
    public class MainThread : IDisposable
    {
        private readonly WindowsUpdateTrayIcon mTrayIcon;
        private readonly WindowsUpdateManager mUpdateManager;

        public MainThread()
        {
            mTrayIcon = new WindowsUpdateTrayIcon(_OnIconClicked);
            mUpdateManager = new WindowsUpdateManager(_OnSearchFinished);

            _SearchForUpdates();
        }

        private void _SearchForUpdates()
        {
            mUpdateManager.StartSearchForUpdates();
            mTrayIcon.SetToolTip(TextResources.Searching);
        }

        private void _OnSearchFinished(int updateCount)
        {
            var message = TextResources.NothingFound;

            if(updateCount > 0)
            {
                message = _GetMessage(updateCount);

                var popup = new Popup();
                popup.DataContext = new PopupViewModel(TextResources.UpdatesAvailable, message, popup.Close);
                popup.Show();
            }

            mTrayIcon.SetToolTip(message);
        }

        private void _OnIconClicked()
        {
            var popup = new Popup();
            popup.DataContext = new PopupViewModel(TextResources.UpdatesAvailable, _GetMessage(1), popup.Close);
            popup.Show();
        }

        private string _GetMessage(int updateCount)
        {
            return updateCount > 1
                ? string.Format(TextResources.UpdatesAvailableMessage, updateCount)
                : TextResources.OneUpdateAvailableMessage;
        }

        public void Dispose()
        {
            mTrayIcon.Dispose();
        }
    }
}