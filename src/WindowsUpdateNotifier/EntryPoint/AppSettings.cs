using System;
using System.Configuration;
using System.IO;

namespace WindowsUpdateNotifier
{
    public class AppSettings
    {
        private const string REFRESH_INTERVAL = "RefreshInterval";
        private const string HIDE_ICON = "HideIcon";

        private static AppSettings sInstance;
        private readonly Configuration mConfig;

        public static AppSettings Instance
        {
            get { return sInstance ?? (sInstance = new AppSettings()); }
        }

        private AppSettings()
        {
            mConfig = _LoadConfigurationFile();

            _AddDefaultValues();

            RefreshInterval = int.Parse(mConfig.AppSettings.Settings[REFRESH_INTERVAL].Value);
            HideIcon = bool.Parse(mConfig.AppSettings.Settings[HIDE_ICON].Value);
        }

        public int RefreshInterval { get; private set; }

        public bool HideIcon { get; private set; }

        public Action OnSettingsChanged { get; set; }

        public void Save(int refreshInterval, bool hideIcon)
        {
            mConfig.AppSettings.Settings[REFRESH_INTERVAL].Value = refreshInterval.ToString();
            mConfig.AppSettings.Settings[HIDE_ICON].Value = hideIcon.ToString();

            mConfig.Save();

            RefreshInterval = refreshInterval;
            HideIcon = hideIcon;

            if (OnSettingsChanged != null)
                OnSettingsChanged();
        }

        private Configuration _LoadConfigurationFile()
        {
            var configFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WindowsUpdateNotifier", "WindowsUpdateNotifier.config");

            var c = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            return ConfigurationManager.OpenMappedExeConfiguration(c, ConfigurationUserLevel.None);
        }

        private void _AddDefaultValues()
        {
            if (mConfig.AppSettings.Contains(REFRESH_INTERVAL) == false)
                mConfig.AppSettings.Settings.Add(REFRESH_INTERVAL, "60");

            if (mConfig.AppSettings.Contains(HIDE_ICON) == false)
                mConfig.AppSettings.Settings.Add(HIDE_ICON, "False");
        }
    }

    public static class  AppSettingsExtensions
    {
        public static bool Contains(this AppSettingsSection config, string key)
        {
            return config.Settings[key] != null;
        }
    }
}