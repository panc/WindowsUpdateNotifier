using System.Windows;

namespace WindowsUpdateNotifier.Desktop
{
    public partial class Popup : Window
    {
        public Popup()
        {
            InitializeComponent();

            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Width + workingArea.Left - Width;
            Top = workingArea.Height + workingArea.Top - Height;
        }
    }
}
