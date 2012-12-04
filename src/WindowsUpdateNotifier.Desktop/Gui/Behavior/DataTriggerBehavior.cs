using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WindowsUpdateNotifier.Desktop
{
    public static class DataTriggerBehavior
    {
        public static readonly DependencyProperty StoryCompletedCommandProperty =
          DependencyProperty.RegisterAttached("StoryCompletedCommand", typeof(ICommand), typeof(DataTriggerBehavior), new PropertyMetadata(_AttachOrRemoveCompletedEvent));

        public static ICommand GetStoryCompletedCommand(Storyboard story)
        {
            return (ICommand)story.GetValue(StoryCompletedCommandProperty);
        }

        public static void SetStoryCompletedCommand(Storyboard story, ICommand value)
        {
            story.SetValue(StoryCompletedCommandProperty, value);
        }

        private static void _AttachOrRemoveCompletedEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var storyboard = dependencyObject as Storyboard;
            if (storyboard == null)
                return;

            var newValue = e.NewValue as ICommand;

            if (e.OldValue == null && newValue != null)
            {
                storyboard.Completed += _OnCompleted;
            }
            else if (e.OldValue != null && newValue == null)
            {
                storyboard.Completed -= _OnCompleted;
            }
        }

        private static void _OnCompleted(object sender, EventArgs e)
        {
            var clockGroup = sender as ClockGroup;

            if (clockGroup == null)
                return;

            var storyboard = clockGroup.Timeline as Storyboard;

            if (storyboard == null)
                return;

            var cmd = storyboard.GetValue(StoryCompletedCommandProperty) as ICommand;
            if (cmd != null && cmd.CanExecute(storyboard))
                cmd.Execute(storyboard);
        }
    }
}