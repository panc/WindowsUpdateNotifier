using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class WindowsUpdateTrayIcon : IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;
        private readonly MenuItem mInfoMenuItem;
        private readonly MenuItem mStartMenuItem;
        private readonly Timer mAnimationTimer;
        private readonly MenuItem mDisableNotificationsMenuItem;
        private int mSearchIconIndex;

        public WindowsUpdateTrayIcon(IApplication application)
        {
            mInfoMenuItem = new MenuItem("") { Enabled = false };
            mStartMenuItem = new MenuItem(TextResources.Menu_StartSearch, (s, e) => application.SearchForUpdates());
            mDisableNotificationsMenuItem = new MenuItem(TextResources.Menu_DisableNotification, (e, s) => _DisableNotifications(application));
            
            var contextMenu = new ContextMenu(new[]
            {
                mInfoMenuItem, 
                new MenuItem("-"),
                mStartMenuItem,
                new MenuItem(TextResources.Menu_WindowsUpdates, (s, e) => application.OpenWindowsUpdateControlPanel()),
                mDisableNotificationsMenuItem, 
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_Exit, _OnExitClicked)
            });

            mNotifyIcon = new NotifyIcon
            {
                ContextMenu = contextMenu,
                Icon = UpdateState.NoUpdatesAvailable.GetIcon(),
                Visible = true,
            };

            mAnimationTimer = new Timer { Interval = 250 };
            mAnimationTimer.Tick += (x, y) => _OnRefreshSearchIcon();
        }

        public void Dispose()
        {
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();

            mAnimationTimer.Enabled = false;
            mAnimationTimer.Dispose();
        }

        public void SetToolTipAndMenuItems(string toolTip, string menuText, UpdateState state)
        {
            mNotifyIcon.Text = toolTip;
            mNotifyIcon.Icon = state.GetIcon();
            mSearchIconIndex = 1;

            mInfoMenuItem.Text = menuText;
            mStartMenuItem.Enabled = state != UpdateState.Searching;
            mAnimationTimer.Enabled = state == UpdateState.Searching;
        }

        private void _OnRefreshSearchIcon()
        {
            var icon = (Icon)ImageResources.ResourceManager.GetObject(string.Format("WindowsUpdateSearching{0}", mSearchIconIndex));
            mNotifyIcon.Icon = icon;

            mSearchIconIndex = mSearchIconIndex == 4 ? 1 : ++mSearchIconIndex;
        }

        private void _OnExitClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void _DisableNotifications(IApplication application)
        {
            var disabled = !mDisableNotificationsMenuItem.Checked;
            mDisableNotificationsMenuItem.Checked = disabled;

            application.NotificationsDisabled = disabled;
        }
    }
}