using System.Windows;
using System.Windows.Input;

namespace WindowsUpdateNotifier.Desktop
{
    public static class WindowBehavior
    {
        public static readonly DependencyProperty EscapeClosesWindowProperty =
            DependencyProperty.RegisterAttached("Window_EscapeClosesWindow", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(false, _AttachOrRemovePreviewKeyDownEventEvent));

        public static bool GetEscapeClosesWindow(Window window)
        {
            return (bool)window.GetValue(EscapeClosesWindowProperty);
        }

        public static void SetEscapeClosesWindow(Window window, bool value)
        {
            window.SetValue(EscapeClosesWindowProperty, value);
        }

        private static void _AttachOrRemovePreviewKeyDownEventEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null)
                return;

            if ((e.NewValue is bool) == false)
                return;

            if ((bool)e.NewValue)
                window.PreviewKeyDown += _OnWindowOnPreviewKeyDown;
            else
                window.PreviewKeyDown -= _OnWindowOnPreviewKeyDown;
        }

        private static void _OnWindowOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
                return;

            if (e.Key == Key.Escape)
                window.Close();
        }


        public static readonly DependencyProperty BackgroundClickCommandProperty =
          DependencyProperty.RegisterAttached("BackgroundClickCommand", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(_AttachOrRemoveDoubleClickEvent));

        public static ICommand GetBackgroundClickCommand(Window window)
        {
            return (ICommand)window.GetValue(BackgroundClickCommandProperty);
        }

        public static void SetBackgroundClickCommand(Window window, ICommand value)
        {
            window.SetValue(BackgroundClickCommandProperty, value);
        }

        private static void _AttachOrRemoveDoubleClickEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null)
                return;

            var newValue = e.NewValue as ICommand;

            if (e.OldValue == null && newValue != null)
            {
                window.MouseUp += _OnMouseUp;
            }
            else if (e.OldValue != null && newValue == null)
            {
                window.MouseUp -= _OnMouseUp;
            }
        }

        private static void _OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var window = sender as Window;

            if (window == null)
                return;

            var cmd = window.GetValue(BackgroundClickCommandProperty) as ICommand;
            if (cmd != null && cmd.CanExecute(window))
                cmd.Execute(window);
        }
    }
}