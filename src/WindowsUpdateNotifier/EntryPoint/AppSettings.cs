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
        private const string DISABLE_NOTIFICATIONS = "DisableNotifications";
        private const string USE_METRO_STYLE = "UseMetroStyle";
        private const string INSTALL_UPDATES = "InstallUpdates";
        private const string KB_IDS_TO_INSTALL = "KbIdsToInstall";

        private const string WINDOWS_7_DEFENDER_KB_ID = "2310138";
        private const string WINDOWS_8_DEFENDER_KB_ID = "2267602";

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
            DisableNotifications = bool.Parse(mConfig.AppSettings.Settings[DISABLE_NOTIFICATIONS].Value);
            UseMetroStyle = bool.Parse(mConfig.AppSettings.Settings[USE_METRO_STYLE].Value);
            InstallUpdates = bool.Parse(mConfig.AppSettings.Settings[INSTALL_UPDATES].Value);

            var kbIds = mConfig.AppSettings.Settings[KB_IDS_TO_INSTALL].Value;
            var ids = kbIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            KbIdsToInstall = ids;
        }

        public int RefreshInterval { get; private set; }

        public bool HideIcon { get; private set; }

        public bool DisableNotifications { get; set; }

        public bool UseMetroStyle { get; private set; }

        public bool InstallUpdates { get; private set; }

        public string[] KbIdsToInstall { get; private set; }

        public Action OnSettingsChanged { get; set; }

        public void Save(int refreshInterval, bool hideIcon, bool disableNotifications, bool useMetroStyle, bool installUpdates/*, string[] kbIdsToInstall */)
        {
            var hasChanged = _SetSetting(REFRESH_INTERVAL, refreshInterval.ToString(CultureInfo.InvariantCulture));
            hasChanged = _SetSetting(HIDE_ICON, hideIcon.ToString()) || hasChanged;
            hasChanged = _SetSetting(USE_METRO_STYLE, useMetroStyle.ToString()) || hasChanged;
            hasChanged = _SetSetting(DISABLE_NOTIFICATIONS, disableNotifications.ToString()) || hasChanged;
            hasChanged = _SetSetting(INSTALL_UPDATES, installUpdates.ToString()) || hasChanged;
            //hasChanged = _SetSetting(KB_IDS_TO_INSTALL, string.Join(";", kbIdsToInstall)) || hasChanged;

            if (hasChanged)
            {
                mConfig.Save();

                RefreshInterval = refreshInterval;
                HideIcon = hideIcon;
                DisableNotifications = disableNotifications;
                UseMetroStyle = useMetroStyle;
                InstallUpdates = installUpdates;
                //KbIdsToInstall = kbIdsToInstall;

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

            if (mConfig.AppSettings.Contains(DISABLE_NOTIFICATIONS) == false)
                mConfig.AppSettings.Settings.Add(DISABLE_NOTIFICATIONS, "False");

            if (mConfig.AppSettings.Contains(USE_METRO_STYLE) == false)
                mConfig.AppSettings.Settings.Add(USE_METRO_STYLE, "TRUE");

            if (mConfig.AppSettings.Contains(INSTALL_UPDATES) == false)
                mConfig.AppSettings.Settings.Add(INSTALL_UPDATES, "False");

            if (mConfig.AppSettings.Contains(KB_IDS_TO_INSTALL) == false)
                mConfig.AppSettings.Settings.Add(KB_IDS_TO_INSTALL, _GetWindowsDefenderKbId());
            else
                _CorrectKbIdsIfNeeded();
        }

        private void _CorrectKbIdsIfNeeded()
        {
            var kbIds = mConfig.AppSettings.Settings[KB_IDS_TO_INSTALL].Value;

            if (kbIds == WINDOWS_8_DEFENDER_KB_ID && IsRunningOnWindows7())
                _SetSetting(KB_IDS_TO_INSTALL, WINDOWS_7_DEFENDER_KB_ID);
        }

        private string _GetWindowsDefenderKbId()
        {
            return IsRunningOnWindows7()
                ? WINDOWS_7_DEFENDER_KB_ID
                : WINDOWS_8_DEFENDER_KB_ID; // use windows 8 kb-id as a default value even if it is not a windows 8 os.
        }

        private bool IsRunningOnWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 &&
                   Environment.OSVersion.Version.Minor == 1;
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