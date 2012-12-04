using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace WindowsUpdateNotifier.Desktop
{
    public class PopupViewModel
    {
        public PopupViewModel(string title, string message, Action onCloseCallback, IApplication application)
        {
            Title = title;
            Message = message;
            OnCloseCommand = new SimpleCommand(onCloseCallback);
            OnOpenSettingsViewCommand = new SimpleCommand(application.OpenSettingsView);
            OnOpenWindowsUpdateControlPanelCommand= new SimpleCommand(application.OpenWindowsUpdateControlPanel);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
            timer.Tick += (s, e) =>
            {
                onCloseCallback();
                timer.Stop();
            };

            timer.Start();
        }

        public ICommand OnCloseCommand { get; set; }
        
        public ICommand OnOpenSettingsViewCommand { get; set; }

        public ICommand OnOpenWindowsUpdateControlPanelCommand { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}