using System;
using System.Windows.Input;
using WindowsUpdateNotifier.Desktop.Common;

namespace WindowsUpdateNotifier.Desktop
{
    public class PopupViewModel
    {
        public PopupViewModel(string title, string message, Action onCloseClicked)
        {
            Title = title;
            Message = message;
            OnCloseCommand = new Command(onCloseClicked);
        }

        public ICommand OnCloseCommand { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}