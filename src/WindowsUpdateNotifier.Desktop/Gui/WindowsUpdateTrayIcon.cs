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

        public WindowsUpdateTrayIcon(IApplication application)
        {
            var contextMenu = new ContextMenu(new[]
            {
                new MenuItem(TextResources.Menu_StartSearch, (s, e) => application.SearchForUpdates()),
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_WindowsUpdates, (s, e) => application.OpenWindowsUpdateControlPanel()),
                //new MenuItem(TextResources.Settings, (s, e) => application.OpenSettingsView()),
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_Exit, _OnExitClicked)
            });

            mNotifyIcon = new NotifyIcon
            {
                ContextMenu = contextMenu,
                Icon = Resources.WindowsUpdate,
                Visible = true,
            };
        }

        public void Dispose()
        {
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();
        }

        public void SetToolTip(string text)
        {
            mNotifyIcon.Text = text;
        }

        private void _OnExitClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}