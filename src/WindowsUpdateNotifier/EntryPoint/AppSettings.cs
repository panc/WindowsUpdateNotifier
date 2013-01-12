using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace WindowsUpdateNotifier
{
    public class AppSettings
    {
        private const string REFRESH_INTERVAL = "RefreshInterval";
        private const string HIDE_ICON = "HideIcon";

        public static AppSettings Instance { get; private set; }

        public static void Initialize(bool withDefaultSettings)
        {
            if (Instance != null)
                throw new InvalidOperationException("AppSettings are already initialzed!");

            Instance = new AppSettings(withDefaultSettings);
        }


        private readonly Configuration mConfig;

        private AppSettings(bool withDefaultSettings)
        {
            mConfig = _LoadConfigurationFile();

            _AddDefaultValues(withDefaultSettings);

            RefreshInterval = int.Parse(mConfig.AppSettings.Settings[REFRESH_INTERVAL].Value);
            HideIcon = bool.Parse(mConfig.AppSettings.Settings[HIDE_ICON].Value);
        }

        public int RefreshInterval { get; private set; }

        public bool HideIcon { get; private set; }

        public Action OnSettingsChanged { get; set; }

        public void Save(int refreshInterval, bool hideIcon)
        {
            var hasIntervalChanged = _SetSetting(REFRESH_INTERVAL, refreshInterval.ToString(CultureInfo.InvariantCulture));
            var hasHideIconChanged = _SetSetting(HIDE_ICON, hideIcon.ToString());

            if (hasIntervalChanged || hasHideIconChanged)
            {
                mConfig.Save();

                RefreshInterval = refreshInterval;
                HideIcon = hideIcon;

                if (OnSettingsChanged != null)
                    OnSettingsChanged();
            }
        }

        private bool _SetSetting(string key, string value)
        {
            if (mConfig.AppSettings.Settings[key].Value != value)
            {
                mConfig.AppSettings.Settings[key].Value = value;
                return true;
            }

            return false;
        }

        private Configuration _LoadConfigurationFile()
        {
            var configFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WindowsUpdateNotifier", "WindowsUpdateNotifier.config");

            var c = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            return ConfigurationManager.OpenMappedExeConfiguration(c, ConfigurationUserLevel.None);
        }

        private void _AddDefaultValues(bool withDefaultSettings)
        {
            if (withDefaultSettings)
                mConfig.AppSettings.Settings.Clear();

            if (mConfig.AppSettings.Contains(REFRESH_INTERVAL) == false)
                mConfig.AppSettings.Settings.Add(REFRESH_INTERVAL, "60");

            if (mConfig.AppSettings.Contains(HIDE_ICON) == false)
                mConfig.AppSettings.Settings.Add(HIDE_ICON, "False");
        }
    }

    public static class AppSettingsExtensions
    {
        public static bool Contains(this AppSettingsSection config, string key)
        {
            return config.Settings[key] != null;
        }
    }
}