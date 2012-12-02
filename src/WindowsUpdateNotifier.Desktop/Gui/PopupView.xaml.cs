using System.Windows;

namespace WindowsUpdateNotifier.Desktop
{
    public partial class PopupView : Window
    {
        public PopupView()
        {
            InitializeComponent();

            var workingArea = SystemParameters.WorkArea;
            Left = workingArea.Width + workingArea.Left - Width;
            Top = workingArea.Height + workingArea.Top - Height;
        }
    }
}
