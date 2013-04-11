namespace WindowsUpdateNotifier
{
    public interface IApplication
    {
        void OpenWindowsUpdateControlPanel();
        void OpenSettings();
        void OpenDownloadPage();
        void OpenAboutDialog(); 
        void SearchForUpdates();
    }
}