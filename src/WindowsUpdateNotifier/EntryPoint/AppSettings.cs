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
        private const string USE_METRO_STYLE = "UseMetroStyle";

        public static AppSettings Instance { get; private set; }

        public static void Initialize(bool useDefaultSettings)
        {
            if (Instance != null)
                throw new InvalidOperationException("AppSettings are already initialzed!");

            Instance = new AppSettings(useDefaultSettings);
        }


        private readonly Configuration mConfig;

        private AppSettings(bool useDefaultSettings)
        {
            mConfig = _LoadConfigurationFile();

            _AddDefaultValues(useDefaultSettings);

            RefreshInterval = int.Parse(mConfig.AppSettings.Settings[REFRESH_INTERVAL].Value);
            HideIcon = bool.Parse(mConfig.AppSettings.Settings[HIDE_ICON].Value);
            UseMetroStyle = bool.Parse(mConfig.AppSettings.Settings[USE_METRO_STYLE].Value);
        }

        public int RefreshInterval { get; private set; }

        public bool HideIcon { get; private set; }

        public bool UseMetroStyle { get; private set; }

        public Action OnSettingsChanged { get; set; }

        public void Save(int refreshInterval, bool hideIcon, bool useMetroStyle)
        {
            var hasIntervalChanged = _SetSetting(REFRESH_INTERVAL, refreshInterval.ToString(CultureInfo.InvariantCulture));
            var hasHideIconChanged = _SetSetting(HIDE_ICON, hideIcon.ToString());
            var hasUseMetroStyleChanged = _SetSetting(USE_METRO_STYLE, useMetroStyle.ToString());

            if (hasIntervalChanged || hasHideIconChanged || hasUseMetroStyleChanged)
            {
                mConfig.Save();

                RefreshInterval = refreshInterval;
                HideIcon = hideIcon;
                UseMetroStyle = useMetroStyle;

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

        private void _AddDefaultValues(bool useDefaultSettings)
        {
            if (useDefaultSettings)
                mConfig.AppSettings.Settings.Clear();

            if (mConfig.AppSettings.Contains(REFRESH_INTERVAL) == false)
                mConfig.AppSettings.Settings.Add(REFRESH_INTERVAL, "60");

            if (mConfig.AppSettings.Contains(HIDE_ICON) == false)
                mConfig.AppSettings.Settings.Add(HIDE_ICON, "False");

            if (mConfig.AppSettings.Contains(USE_METRO_STYLE) == false)
                mConfig.AppSettings.Settings.Add(USE_METRO_STYLE, "TRUE");
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