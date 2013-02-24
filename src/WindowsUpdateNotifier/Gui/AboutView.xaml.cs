using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();

            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Width + workingArea.Left - Width - 10;
            Top = workingArea.Height + workingArea.Top - Height - 10;
        }
    }
}
