using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class SystemTrayIcon : IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;
        private readonly MenuItem mVersionMenuItem;
        private readonly MenuItem mInfoMenuItem;
        private readonly MenuItem mStartMenuItem;
        private readonly Timer mAnimationTimer;
        private int mSearchIconIndex;
        private readonly BalloonTipHelper mBallonTipHelper;

        public SystemTrayIcon(IApplication application)
        {
            mVersionMenuItem = new MenuItem("", (s, e) => application.OpenDownloadPage()) { DefaultItem = true, Visible = false };
            mInfoMenuItem = new MenuItem("") { Enabled = false };
            mStartMenuItem = new MenuItem(TextResources.Menu_StartSearch, (s, e) => application.SearchForUpdates());
            
            var contextMenu = new ContextMenu(new[]
            {
                mVersionMenuItem,
                mInfoMenuItem, 
                new MenuItem("-"),
                mStartMenuItem,
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_Settings, (s, e) => application.OpenSettings()),
                new MenuItem(TextResources.Menu_WindowsUpdates, (s, e) => application.OpenWindowsUpdateControlPanel()),
                new MenuItem("-"),
                new MenuItem(TextResources.Menu_Exit, _OnExitClicked)
            });

            mNotifyIcon = new NotifyIcon
            {
                ContextMenu = contextMenu,
                Icon = UpdateState.NoUpdatesAvailable.GetIcon(),
                Visible = true,
            };

            mNotifyIcon.MouseUp += (s, e) => _OnMouseUp(application, e);
            mNotifyIcon.BalloonTipClicked += (s, e) => application.OpenWindowsUpdateControlPanel();

            mAnimationTimer = new Timer { Interval = 250 };
            mAnimationTimer.Tick += (x, y) => _OnRefreshSearchIcon();

            mBallonTipHelper = new BalloonTipHelper(mNotifyIcon);
        }

        public void Dispose()
        {
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();

            mAnimationTimer.Enabled = false;
            mAnimationTimer.Dispose();
        }

        public void SetupToolTipAndMenuItems(string toolTip, string menuText, UpdateState state)
        {
            mNotifyIcon.Text = _GetStringWithMaxLength(toolTip, 60);
            mInfoMenuItem.Text = _GetStringWithMaxLength(menuText, 60);
            mStartMenuItem.Enabled = state != UpdateState.Searching;
        }

        public void SetIcon(UpdateState state)
        {
            mSearchIconIndex = 1;
            mNotifyIcon.Icon = state.GetIcon();

            mNotifyIcon.Visible = (state != UpdateState.UpdatesAvailable && AppSettings.Instance.HideIcon) == false;
            mAnimationTimer.Enabled = state == UpdateState.Searching && AppSettings.Instance.HideIcon == false;
        }

        public void ShowBallonTip(string title, string message, UpdateState state)
        {
            mBallonTipHelper.ShowBalloon(1, title, message, 15000, state.GetPopupIcon());
        }

        public void SetVersionMenuItem(string version)
        {
            mVersionMenuItem.Text = string.Format(TextResources.Menu_NewVersion, version);
            mVersionMenuItem.Visible = true;
        }

        private void _OnRefreshSearchIcon()
        {
            var icon = (Icon)ImageResources.ResourceManager.GetObject(string.Format("WindowsUpdateSearching{0}", mSearchIconIndex));
            mNotifyIcon.Icon = icon;

            mSearchIconIndex = mSearchIconIndex == 4 ? 1 : ++mSearchIconIndex;
        }

        private void _OnMouseUp(IApplication application, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                application.OpenAboutDialog();
        }
        
        private void _OnExitClicked(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private string _GetStringWithMaxLength(string text, int length)
        {
            return text.Length > length ? text.Substring(0, length) : text;
        }
    }
}