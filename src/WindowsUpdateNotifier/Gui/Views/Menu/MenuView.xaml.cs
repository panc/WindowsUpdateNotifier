using System.Windows;
using System.Windows.Forms;

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
            var l = Control.MousePosition;
            var location = PointFromScreen(new Point(l.X, l.Y));

            Left = location.X - (Width / 2);
            Top = location.Y - Height - 25;
        }
    }
}
