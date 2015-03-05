using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WindowsUpdateNotifier
{
    public static class DataTriggerBehavior
    {
        public static readonly DependencyProperty StoryboardCompletedCommandProperty =
          DependencyProperty.RegisterAttached("StoryboardCompletedCommand", typeof(ICommand), typeof(DataTriggerBehavior), new PropertyMetadata(_AttachOrRemoveCompletedEvent));

        public static ICommand GetStoryboardCompletedCommand(Storyboard story)
        {
            return (ICommand)story.GetValue(StoryboardCompletedCommandProperty);
        }

        public static void SetStoryboardCompletedCommand(Storyboard story, ICommand value)
        {
            story.SetValue(StoryboardCompletedCommandProperty, value);
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

            var cmd = storyboard.GetValue(StoryboardCompletedCommandProperty) as ICommand;
            if (cmd != null && cmd.CanExecute(storyboard))
                cmd.Execute(storyboard);
        }
    }
}