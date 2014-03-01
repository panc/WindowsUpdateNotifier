using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class SystemTrayIcon : IDisposable
    {
        private readonly NotifyIcon mNotifyIcon;
        private readonly Timer mAnimationTimer;
        private readonly BalloonTipHelper mBallonTipHelper;

        private int mSearchIconIndex;

        public SystemTrayIcon(IApplication application)
        {
            mNotifyIcon = new NotifyIcon
            {
                Icon = UpdateState.NoUpdatesAvailable.GetIcon(),
                Visible = true
            };

            mNotifyIcon.MouseUp += (s, e) => application.OpenMenuDialog();
            mNotifyIcon.BalloonTipClicked += (s, e) => application.OpenWindowsUpdateControlPanel();

            mAnimationTimer = new Timer { Interval = 250 };
            mAnimationTimer.Tick += (x, y) => _RefreshSearchIcon();

            mBallonTipHelper = new BalloonTipHelper(mNotifyIcon);
        }

        public void Dispose()
        {
            mNotifyIcon.Visible = false;
            mNotifyIcon.Dispose();

            mAnimationTimer.Enabled = false;
            mAnimationTimer.Dispose();
        }

        public void SetupToolTip(string toolTip)
        {
            mNotifyIcon.Text = _GetStringWithMaxLength(toolTip, 60);
        }

        private string _GetStringWithMaxLength(string text, int length)
        {
            return text.Length > length ? text.Substring(0, length) : text;
        }

        public void SetIcon(UpdateState state, int availableUpdates = 0)
        {
            mSearchIconIndex = 1;
            mNotifyIcon.Icon = state.GetIcon(availableUpdates);

            mNotifyIcon.Visible = (state != UpdateState.UpdatesAvailable && AppSettings.Instance.HideIcon) == false;
            mAnimationTimer.Enabled = state == UpdateState.Searching && AppSettings.Instance.HideIcon == false;
        }

        public void ShowBallonTip(string title, string message, UpdateState state)
        {
            mBallonTipHelper.ShowBalloon(title, message, 15000, state.GetPopupIcon());
        }

        private void _RefreshSearchIcon()
        {
            var icon = (Icon)ImageResources.ResourceManager.GetObject(string.Format("WindowsUpdateSearching{0}", mSearchIconIndex));
            mNotifyIcon.Icon = icon;

            mSearchIconIndex = mSearchIconIndex == 4 ? 1 : ++mSearchIconIndex;
        }
    }
}