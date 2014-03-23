using System.Globalization;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class MenuView
    {
        private const int OFFSET = 10;

        public MenuView()
        {
            InitializeComponent();
        }

        private void _OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var workingArea = SystemParameters.WorkArea;
            var taskBarLocation = _GetTaskBarLocation(workingArea);
            var isLeftToRight = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            if (taskBarLocation == TaskBarLocation.Top || taskBarLocation == TaskBarLocation.Bottom)
            {
                Left = (isLeftToRight)
                   ? workingArea.Left + OFFSET
                   : workingArea.Width + workingArea.Left - Width - OFFSET;
                
                Top = (taskBarLocation == TaskBarLocation.Bottom)
                    ? workingArea.Height - ActualHeight - OFFSET
                    : workingArea.Top + OFFSET;
            }
            else
            {
                Left = (taskBarLocation == TaskBarLocation.Right)
                    ? workingArea.Width - ActualWidth - OFFSET
                    : workingArea.Left + OFFSET;

                Top = (isLeftToRight)
                    ? workingArea.Top + OFFSET
                    : workingArea.Height + workingArea.Top - ActualHeight - OFFSET;
            }
        }
        
        private TaskBarLocation _GetTaskBarLocation(Rect workingArea)
        {
            if (workingArea.Left > 0)
                return TaskBarLocation.Left;
            if (workingArea.Top > 0)
                return TaskBarLocation.Top;
            if (workingArea.Left == 0 && workingArea.Width < SystemParameters.PrimaryScreenWidth)
                return TaskBarLocation.Right;
            
            return TaskBarLocation.Bottom;
        }

        private enum TaskBarLocation { Top, Bottom, Left, Right }
    }
}
