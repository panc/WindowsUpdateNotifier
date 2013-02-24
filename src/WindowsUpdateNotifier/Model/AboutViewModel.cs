using System;
using System.Windows.Input;

namespace WindowsUpdateNotifier
{
    public class AboutViewModel
    {
        private readonly VersionHelper mVersionHelper;

        public AboutViewModel()
        {
        }

        public AboutViewModel(VersionHelper versionHelper, Action onCloseCallback, Action openUpdatePage)
        {
            mVersionHelper = versionHelper;
            mVersionHelper.RegisterForNotification(_OnNewVersionAvailable);

            OnCloseCommand = new SimpleCommand(onCloseCallback);
            OnOpenUpdatePageCommand= new SimpleCommand(openUpdatePage);
        }

        private void _OnNewVersionAvailable(string newVersion)
        {
            
        }

        public ICommand OnCloseCommand { get; set; }
        
        public ICommand OnOpenUpdatePageCommand { get; set; }

        public string VersionLabel { get; set; }

        public string CopyrightLabel { get; set; }
    }
}