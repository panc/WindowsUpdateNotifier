using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WindowsUpdateNotifier
{
    public class AboutViewModel
    {
        private readonly VersionHelper mVersionHelper;

        public AboutViewModel()
        {
        }

        public AboutViewModel(VersionHelper versionHelper, Action openUpdatePage)
        {
            mVersionHelper = versionHelper;
            mVersionHelper.RegisterForNotification(_OnNewVersionAvailable);

            var version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            HomepageLink = "http://wun.codeplex.com";
            CopyrightLabel = version.LegalCopyright;
            VersionLabel = string.Format("Version {0}", version.ProductVersion);

            OpenUpdatePageCommand = new SimpleCommand(openUpdatePage);
            OpenHomepageCommand = new SimpleCommand(_OpenHomepage);
        }

        private void _OpenHomepage()
        {
            Process.Start(HomepageLink);
        }

        private void _OnNewVersionAvailable(string newVersion)
        {
            
        }

        public ICommand OpenUpdatePageCommand { get; set; }

        public ICommand OpenHomepageCommand { get; set; }

        public string VersionLabel { get; set; }

        public string CopyrightLabel { get; set; }

        public string HomepageLink { get; set; }
    }
}