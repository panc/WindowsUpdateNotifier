using System;
using System.Windows.Input;
using System.Windows.Media;

namespace WindowsUpdateNotifier
{
    public class PopupViewModel : ViewModel
    {
        public PopupViewModel()
        {
        }

        public PopupViewModel(string title, string message, UpdateState state, Action onCloseCallback, Action openWindowsUpdateControlPanel)
        {
            Image = state.GetPopupImage();
            Title = title;
            Message = message;
            BackgroundColor = new SolidColorBrush(ColorHelper.GetWindowsThemeBackgroundColor());

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

        public bool StartHiding { get; set; }

        public Brush BackgroundColor { get; set; }
    }
}