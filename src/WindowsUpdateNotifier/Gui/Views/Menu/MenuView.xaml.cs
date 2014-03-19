using System.Globalization;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class MenuView
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void _OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var workingArea = SystemParameters.WorkArea;
            Left = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
                ? workingArea.Left + 10
                : workingArea.Width + workingArea.Left - Width - 10;

            Top = workingArea.Height + workingArea.Top - ActualHeight - 10;
        }
    }
}
