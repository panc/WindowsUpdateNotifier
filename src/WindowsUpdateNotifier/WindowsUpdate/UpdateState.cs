using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public enum UpdateState
    {
        Searching,
        UpdatesAvailable,
        NoUpdatesAvailable,
        UpdatesInstalled,
        Failure
    }

    public static class UpdateStateExtensions
    {
        private static readonly Font FONT = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);
        private static readonly SolidBrush BRUSH = new SolidBrush(System.Drawing.Color.White);
        private static readonly StringFormat FORMAT = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public static bool CanApplicationBeClosed(this UpdateState state, int failureCount)
        {
            switch (state)
            {
                case UpdateState.Failure:
                    return failureCount >= 10;
                case UpdateState.Searching:
                case UpdateState.UpdatesAvailable:
                    return false;
                default:
                    return true;
            }
        }
        
        public static Icon GetIcon(this UpdateState state, int availableUpdates = 0)
        {
            switch (state)
            {
                case UpdateState.Searching:
                    return ImageResources.WindowsUpdateSearching1;
                case UpdateState.UpdatesAvailable:
                    return _CreateIconWithNumber(availableUpdates);
                case UpdateState.Failure:
                    return ImageResources.WindowsUpdateNoConnection;
                default:
                    return ImageResources.WindowsUpdateNothing;
            }
        }

        private static Icon _CreateIconWithNumber(int availableUpdates)
        {
            var bitmap = ImageResources.WindowsUpdateWithNumber.ToBitmap();
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var x = bitmap.Width / 2;
                var y = bitmap.Height / 2;

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.DrawString(availableUpdates.ToString(), FONT, BRUSH, x, y, FORMAT);
            }

            var iconHandle = bitmap.GetHicon();
            return Icon.FromHandle(iconHandle);
        }

        public static Icon GetPopupIcon(this UpdateState state)
        {
            switch (state)
            {
                case UpdateState.UpdatesAvailable:
                    return ImageResources.ShieldYellow;
                default:
                    return ImageResources.ShieldGreen;
            }
        }
    }
}