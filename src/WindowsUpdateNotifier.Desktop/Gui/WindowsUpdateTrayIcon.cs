using System;
using System.Windows.Forms;
using WindowsUpdateNotifier.Core.Resources;
using WindowsUpdateNotifier.Desktop.Properties;

namespace WindowsUpdateNotifier
{
    public class WindowsUpdateTrayIcon : IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;

        public WindowsUpdateTrayIcon(Action onOpenSettingsClicked)
        {
            var contextMenu = new ContextMenu(new[]
            {
                new MenuItem(TextResources.Settings, (s, e) => onOpenSettingsClicked()),
                new MenuItem(TextResources.Exit, _OnExitClicked)
            });

            mNotifyIcon = new NotifyIcon
            {
                ContextMenu = contextMenu,
                Icon = Resources.WindowsUpdate,
                Text = TextResources.ToolTip,
                Visible = true,
            };

            // temp
            mNotifyIcon.Click += (x, y) => onOpenSettingsClicked();
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