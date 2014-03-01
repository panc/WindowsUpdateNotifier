using System;
using System.Diagnostics;
using System.Windows.Input;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class MenuViewModel : ViewModel
    {
        private string mUpdateStateText;
        private bool mIsSearchForUpdatesEnabled;

        public MenuViewModel()
        {
        }

        public MenuViewModel(IApplication application, IVersionHelper versionHelper)
        {
            HomepageLink = "http://wun.codeplex.com";
            UpdateStateText = TextResources.ToolTip_NothingFound;
            SetVersionInfo(versionHelper);

            OpenDownloadPageCommand = new SimpleCommand(application.OpenDownloadPage);
            OpenHomepageCommand = new SimpleCommand(_OpenHomepage);
            OpenSettingsCommand = new SimpleCommand(application.OpenSettings);
            OpenWindowsUpdateControlPanelCommand = new SimpleCommand(application.OpenWindowsUpdateControlPanel);
            SearchForUpdatesCommand = new SimpleCommand(application.SearchForUpdates);
            ShutdownCommand = new SimpleCommand(application.Shutdown);
        }

        public void SetVersionInfo(IVersionHelper versionHelper)
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
            try
            {
                Process.Start(HomepageLink);
            }
            catch(Exception)
            {
                // do nothing
            }
        }

        public ICommand OpenDownloadPageCommand { get; set; }

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

        public bool IsSearchForUpdatesEnabled
        {
            get { return mIsSearchForUpdatesEnabled; }
            set
            {
                mIsSearchForUpdatesEnabled = value;
                RaisePropertyChanged(() => IsSearchForUpdatesEnabled);
            }
        }

        public string UpdateStateText
        {
            get { return mUpdateStateText; }
            set
            {
                mUpdateStateText = value;
                RaisePropertyChanged(() => UpdateStateText);
            }
        }
    }
}