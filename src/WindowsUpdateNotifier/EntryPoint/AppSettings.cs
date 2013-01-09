namespace WindowsUpdateNotifier
{
    public class AppSettings
    {
        private static AppSettings sInstance;

        public static AppSettings Instance
        {
            get { return sInstance ?? (sInstance = new AppSettings()); }
        }

        private AppSettings()
        {
            RefreshInterval = 60;
            HideIcon = false;
        }

        public int RefreshInterval { get; set; }

        public bool HideIcon { get; set; }

        public void Save()
        {
            // todo: save to registry
        } 
    }
}