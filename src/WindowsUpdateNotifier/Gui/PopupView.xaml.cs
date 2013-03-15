using System.Globalization;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class PopupView : Window
    {
        public PopupView()
        {
            InitializeComponent();

            var workingArea = SystemParameters.WorkArea;
            Left = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
                ? workingArea.Left + 10
                : workingArea.Width + workingArea.Left - Width - 10;

            Top = workingArea.Height + workingArea.Top - Height - 10;
        }
    }
}
