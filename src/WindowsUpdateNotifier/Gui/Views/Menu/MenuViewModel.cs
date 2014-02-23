using System.Diagnostics;
using System.Windows.Input;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
        }

        public MenuViewModel(IApplication application, VersionHelper versionHelper)
        {
            HomepageLink = "http://wun.codeplex.com";
            SetVersionInfo(versionHelper);

            OpenUpdatePageCommand = new SimpleCommand(application.OpenDownloadPage);
            OpenHomepageCommand = new SimpleCommand(_OpenHomepage);
            OpenSettingsCommand = new SimpleCommand(application.OpenSettings);
            OpenWindowsUpdateControlPanelCommand = new SimpleCommand(application.OpenWindowsUpdateControlPanel);
            SearchForUpdatesCommand = new SimpleCommand(application.SearchForUpdates);
            ShutdownCommand = new SimpleCommand(application.Shutdown);
        }

        public void SetVersionInfo(VersionHelper versionHelper)
        {
            CopyrightLabel = versionHelper.Copyright;
            VersionLabel = string.Format("Version {0}", versionHelper.CurrentVersion);

            IsNewVersionAvailable = versionHelper.IsNewVersionAvailable;
            IsLatestVersion = !IsNewVersionAvailable;

            NewVersionLabel = versionHelper.IsNewVersionAvailable
                ? string.Format(TextResources.Label_NewVersion, versionHelper.LatestVersion.Version)
                : TextResources.Label_IsLatestVersion;
        }

        private void _OpenHomepage()
        {
            Process.Start(HomepageLink);
        }

        public ICommand OpenUpdatePageCommand { get; set; }

        public ICommand OpenHomepageCommand { get; set; }

        public ICommand SearchForUpdatesCommand { get; set; }

        public ICommand OpenSettingsCommand { get; set; }

        public ICommand OpenWindowsUpdateControlPanelCommand { get; set; }

        public ICommand ShutdownCommand { get; set; }
        
        public string VersionLabel { get; set; }

        public string NewVersionLabel { get; set; }
        
        public bool IsNewVersionAvailable { get; set; }

        public bool IsLatestVersion { get; set; }

        public string CopyrightLabel { get; set; }

        public string HomepageLink { get; set; }

        public bool IsSearchForUpdatesEnabled { get; set; }

        public string UpdateStateText { get; set; }
    }
}