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
            HomepageLink = "http://wun.codeplex.com";
            CopyrightLabel = versionHelper.Copyright;
            VersionLabel = string.Format("Version {0}", versionHelper.CurrentVersion);

            IsNewVersionAvailable = versionHelper.IsNewVersionAvailable;
            NewVersionLabel = versionHelper.IsNewVersionAvailable
                ? string.Format(TextResources.Label_NewVersion, versionHelper.LatestVersion.Version)
                : TextResources.Label_IsLatestVersion;

            OpenUpdatePageCommand = new SimpleCommand(openUpdatePage);
            OpenHomepageCommand = new SimpleCommand(_OpenHomepage);
        }

        private void _OpenHomepage()
        {
            Process.Start(HomepageLink);
        }

        public ICommand OpenUpdatePageCommand { get; set; }

        public ICommand OpenHomepageCommand { get; set; }

        public string VersionLabel { get; set; }

        public string NewVersionLabel { get; set; }
        
        public bool IsNewVersionAvailable { get; set; }

        public string CopyrightLabel { get; set; }

        public string HomepageLink { get; set; }
    }
}