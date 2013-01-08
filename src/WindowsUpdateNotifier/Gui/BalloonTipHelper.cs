using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class BalloonTipHelper
    {
        private readonly IntPtr mNotifyIconHWnd;

        public BalloonTipHelper(NotifyIcon notifyIcon)
        {
            mNotifyIconHWnd = _GetHandler(notifyIcon);
        }

        private IntPtr _GetHandler(NotifyIcon icon)
        {
            var fieldInfo = icon.GetType().GetField("window", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var nativeWindow = (NativeWindow)fieldInfo.GetValue(icon);

            return nativeWindow.Handle == IntPtr.Zero
                ? IntPtr.Zero
                : nativeWindow.Handle;
        }

        public void ShowBalloon(uint iconId, string title, string text, uint timeout)
        {
            var icon = Icon.ExtractAssociatedIcon(@"D:\Projects\WindowsUpdateNotifier\src\WindowsUpdateNotifier\Resources\shield_yellow_48.ico");
            var icon2 = Icon.ExtractAssociatedIcon(@"D:\Projects\WindowsUpdateNotifier\src\WindowsUpdateNotifier\Resources\WUNotify_32.ico");
            //icon = SystemIcons.WinLogo;

            // show the balloon
            var data = new NotifyIconData
            {
                cbSize = 504,//(UInt32)Marshal.SizeOf(typeof(NotifyIconData)),
                hWnd = mNotifyIconHWnd,
                uID = 1,
                hIcon = icon.Handle,
                //hIcon = ImageResources.WindowsUpdateNoConnection.Handle,
                uFlags = NotifyFlags.Info | NotifyFlags.Icon | NotifyFlags.Message | NotifyFlags.Tip,
                uTimeoutOrVersion = timeout,
                szInfo = text,
                szInfoTitle = title,
                dwInfoFlags = DwInfoFlags.User | DwInfoFlags.LargeIcon,
                hBalloonIcon = icon2.Handle
            };
            
            var i = Shell_NotifyIcon(NotifyCommand.Modify, ref data);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData
        {
            public UInt32 cbSize; // DWORD
            public IntPtr hWnd; // HWND
            public UInt32 uID; // UINT
            public NotifyFlags uFlags; // UINT
            public UInt32 uCallbackMessage; // UINT
            public IntPtr hIcon; // HICON
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String szTip; // char[128]
            public UInt32 dwState; // DWORD
            public UInt32 dwStateMask; // DWORD
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String szInfo; // char[256]
            public UInt32 uTimeoutOrVersion; // UINT
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String szInfoTitle; // char[64]
            public DwInfoFlags dwInfoFlags; // DWORD
            public IntPtr hBalloonIcon;
            //GUID guidItem; > IE 6
        }

        public enum NotifyCommand
        {
            Add = 0, Modify = 1, Delete = 2, SetFocus = 3, SetVersion = 4
        }

        public enum NotifyFlags
        {
            Message = 1, Icon = 2, Tip = 4, State = 8, Info = 16, Guid = 32
        }

        public enum DwInfoFlags
        {
            None = 0, Info = 1, Warning = 2, Error = 3, User = 4, NoSound = 16, LargeIcon= 32
        }

        [DllImport("shell32.Dll")]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, ref NotifyIconData data);
    }
}