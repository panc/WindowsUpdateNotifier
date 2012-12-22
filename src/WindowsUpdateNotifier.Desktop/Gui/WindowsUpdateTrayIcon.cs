using System;
using System.Windows.Forms;
using WindowsUpdateNotifier.Core.Resources;
using WindowsUpdateNotifier.Desktop;
using WindowsUpdateNotifier.Desktop.Properties;

namespace WindowsUpdateNotifier
{
    public class WindowsUpdateTrayIcon : IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;
        private readonly MenuItem mInfoMenuItem;
        private readonly MenuItem mStartMenuItem;

        public WindowsUpdateTrayIcon(IApplication application)
        {
            mInfoMenuItem = new MenuItem("") {Enabled = false};
            mStartMenuItem = new MenuItem(TextResources.Menu_StartSearch, (s, e) => application.SearchForUpdates());

            var contextMenu = new ContextMenu(new[]
            {
                mInfoMenuItem, 
                new MenuItem("-"),
                mStartMenuItem,
                new MenuItem(TextResources.Menu_WindowsUpdates, (s, e) => application.OpenWindowsUpdateControlPanel()),
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_Exit, _OnExitClicked)
            });

            mNotifyIcon = new NotifyIcon
            {
                ContextMenu = contextMenu,
                Icon = Resources.WindowsUpdateNo,
                Visible = true,
            };
        }

        public void Dispose()
        {
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();
        }

        public void SetToolTipAndMenuItems(string text, bool isStartEntryEnabled, bool updatesAvailable)
        {
            mNotifyIcon.Text = text;
            mInfoMenuItem.Text = text;
            mStartMenuItem.Enabled = isStartEntryEnabled;

            if (isStartEntryEnabled == false)
                mNotifyIcon.Icon = Resources.WindowsUpdateSearching;
            else if (updatesAvailable)
                mNotifyIcon.Icon = Resources.WindowsUpdate;
            else
                mNotifyIcon.Icon = Resources.WindowsUpdateNo;
        }

        private void _OnExitClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}