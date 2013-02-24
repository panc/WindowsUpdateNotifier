namespace WindowsUpdateNotifier
{
    public interface IApplication
    {
        void OpenWindowsUpdateControlPanel();
        void OpenSettings();
        void SearchForUpdates();
        
        bool NotificationsDisabled { get; set; }
        void GoToDownloadPage();
        void OpenAboutDialog();
    }
}