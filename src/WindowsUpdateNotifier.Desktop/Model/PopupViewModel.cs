using System;
using System.Windows.Input;

namespace WindowsUpdateNotifier.Desktop
{
    public class PopupViewModel
    {
        public PopupViewModel(string title, string message, Action onCloseClicked, IApplication application)
        {
            Title = title;
            Message = message;
            OnCloseCommand = new SimpleCommand(onCloseClicked);
            OnOpenSettingsViewCommand = new SimpleCommand(application.OpenSettingsView);
            OnOpenWindowsUpdateControlPanelCommand= new SimpleCommand(application.OpenWindowsUpdateControlPanel);
        }

        public ICommand OnCloseCommand { get; set; }
        
        public ICommand OnOpenSettingsViewCommand { get; set; }

        public ICommand OnOpenWindowsUpdateControlPanelCommand { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}