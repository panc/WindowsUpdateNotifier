namespace WindowsUpdateNotifier
{
    public interface IApplication
    {
        void OpenWindowsUpdateControlPanel();
        void OpenSettings();
        void OpenDownloadPage();
        void OpenMenuDialog(); 
        void SearchForUpdates();
        void Shutdown();
    }
}