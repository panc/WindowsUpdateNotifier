namespace WindowsUpdateNotifier
{
    public class MockApplication : IApplication
    {
        public int OpenWindowsUpdateControlPanelCount { get; private set; }
        public int OpenSettingsCount { get; private set; }
        public int OpenDownloadPageCount { get; private set; }
        public int OpenMenuDialogCount { get; private set; }
        public int SearchForUpdatesCount { get; private set; }
        public int ShutdownCount { get; private set; }

        public void OpenWindowsUpdateControlPanel()
        {
            OpenWindowsUpdateControlPanelCount++;
        }
        
        public void OpenSettings()
        {
            OpenSettingsCount++;
        }

        public void OpenDownloadPage()
        {
            OpenDownloadPageCount++;
        }

        public void OpenMenuDialog()
        {
            OpenMenuDialogCount++;
        }

        public void SearchForUpdates()
        {
            SearchForUpdatesCount++;
        }

        public void Shutdown()
        {
            ShutdownCount++;
        }
    }
}