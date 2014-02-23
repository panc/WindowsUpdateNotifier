using System;
using System.Windows.Input;
using System.Windows.Media;

namespace WindowsUpdateNotifier
{
    public class PopupViewModel
    {
        public PopupViewModel()
        {
        }

        public PopupViewModel(string title, string message, UpdateState state, Action onCloseCallback, Action openWindowsUpdateControlPanel)
        {
            Image = state.GetPopupImage();
            Title = title;
            Message = message;
            OnCloseCommand = new SimpleCommand(onCloseCallback);
            OnOpenWindowsUpdateControlPanelCommand= new SimpleCommand(() =>
            {
                openWindowsUpdateControlPanel();
                onCloseCallback();
            });
        }

        public ICommand OnCloseCommand { get; set; }
        
        public ICommand OnOpenWindowsUpdateControlPanelCommand { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public ImageSource Image { get; set; }

        public Boolean StartHiding { get; set; }
    }
}