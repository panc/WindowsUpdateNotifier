using System;
using System.Diagnostics;
using System.Windows.Input;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
        }

        public AboutViewModel(VersionHelper versionHelper, Action openUpdatePage)
        {
            var version = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            HomepageLink = "http://wun.codeplex.com";
            CopyrightLabel = version.LegalCopyright;
            VersionLabel = string.Format("Version {0}", version.ProductVersion);
            
            NewVersionLabel = versionHelper.IsNewVersionAvailable
                ? versionHelper.LatestVersion.Version
                : TextResources.Label_IsLatestVersion;

            OpenUpdatePageCommand = new SimpleCommand(openUpdatePage);
            OpenHomepageCommand = new SimpleCommand(_OpenHomepage);
        }

        private void _OpenHomepage()
        {
            Process.Start(HomepageLink);
        }

        private void _OnNewVersionAvailable(RssVersionItem newVersion)
        {
            NewVersionLabel = string.Format(TextResources.Label_NewVersion, newVersion.Title);
        }

        public ICommand OpenUpdatePageCommand { get; set; }

        public ICommand OpenHomepageCommand { get; set; }

        public string VersionLabel { get; set; }

        public string NewVersionLabel { get; set; }

        public string CopyrightLabel { get; set; }

        public string HomepageLink { get; set; }
    }
}