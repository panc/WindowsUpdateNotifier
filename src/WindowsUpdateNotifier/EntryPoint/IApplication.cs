namespace WindowsUpdateNotifier
{
    public interface IApplication
    {
        void OpenWindowsUpdateControlPanel();
        void OpenSettings();
        void SearchForUpdates();
        void GoToDownloadPage();
        
        bool NotificationsDisabled { get; set; }
    }
}