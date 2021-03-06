﻿using System;
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
        private const string KB_IDS_TO_IGNORE = "KbIdsToIgnore";

        private const string WINDOWS_7_DEFENDER_KB_ID = "2310138;915597";
        private const string WINDOWS_8_DEFENDER_KB_ID = "2267602";
        private const string WINDOWS_10_UPGRADE_KB_ID = "3012973";
        
        public static AppSettings Instance { get; private set; }

        public static void Initialize(bool useDefaultSettings, string settingsFile)
        {
            if (Instance != null)
                throw new InvalidOperationException("AppSettings are already initialzed!");

            Instance = new AppSettings(useDefaultSettings, settingsFile);
        }


        private readonly Configuration mConfig;

        private AppSettings(bool useDefaultSettings, string settingsFile)
        {
            mConfig = _LoadConfigurationFile(settingsFile);

            _AddDefaultValues(useDefaultSettings);

            RefreshInterval = int.Parse(mConfig.AppSettings.Settings[REFRESH_INTERVAL].Value);
            HideIcon = bool.Parse(mConfig.AppSettings.Settings[HIDE_ICON].Value);
            DisableNotifications = bool.Parse(mConfig.AppSettings.Settings[DISABLE_NOTIFICATIONS].Value);
            UseMetroStyle = bool.Parse(mConfig.AppSettings.Settings[USE_METRO_STYLE].Value);
            InstallUpdates = bool.Parse(mConfig.AppSettings.Settings[INSTALL_UPDATES].Value);

            var kbIds = mConfig.AppSettings.Settings[KB_IDS_TO_INSTALL].Value;
            KbIdsToInstall = _ParseKbIds(kbIds);

            var windowsDefenderIds = _GetWindowsDefenderKbId();
            AdditionalKbIds = kbIds.Replace(windowsDefenderIds + ";", "");
            WindowsDefenderKbIds = _ParseKbIds(windowsDefenderIds);

            KbIdsToIgnore = _ParseKbIds(mConfig.AppSettings.Settings[KB_IDS_TO_IGNORE].Value);
        }

        public int RefreshInterval { get; private set; }

        public bool HideIcon { get; private set; }

        public bool DisableNotifications { get; set; }

        public bool UseMetroStyle { get; private set; }

        public bool InstallUpdates { get; private set; }

        public string AdditionalKbIds { get; private set; }

        public string[] KbIdsToInstall { get; private set; }

        public string[] KbIdsToIgnore { get; private set; }
        
        public string[] WindowsDefenderKbIds { get; private set; }

        public Action OnSettingsChanged { get; set; }

        public void Save(int refreshInterval, bool hideIcon, bool disableNotifications, bool useMetroStyle, bool installUpdates, string additionalKbIds)
        {
            var windowsDefenderKbIds = _GetWindowsDefenderKbId();
            var kbIdsToInstall = string.Format("{0};{1}", windowsDefenderKbIds, additionalKbIds);

            var hasChanged = _SetSetting(REFRESH_INTERVAL, refreshInterval.ToString(CultureInfo.InvariantCulture));
            hasChanged = _SetSetting(HIDE_ICON, hideIcon.ToString()) || hasChanged;
            hasChanged = _SetSetting(USE_METRO_STYLE, useMetroStyle.ToString()) || hasChanged;
            hasChanged = _SetSetting(DISABLE_NOTIFICATIONS, disableNotifications.ToString()) || hasChanged;
            hasChanged = _SetSetting(INSTALL_UPDATES, installUpdates.ToString()) || hasChanged;
            hasChanged = _SetSetting(KB_IDS_TO_INSTALL, kbIdsToInstall) || hasChanged;

            if (hasChanged)
            {
                mConfig.Save();

                RefreshInterval = refreshInterval;
                HideIcon = hideIcon;
                DisableNotifications = disableNotifications;
                UseMetroStyle = useMetroStyle;
                InstallUpdates = installUpdates;
                KbIdsToInstall = _ParseKbIds(kbIdsToInstall);
                AdditionalKbIds = additionalKbIds;

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

        private Configuration _LoadConfigurationFile(string configFile)
        {
            Configuration config = null;

            if (!string.IsNullOrEmpty(configFile) && _TryLoadConfigurationFile(configFile, out config))
                return config;

            configFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WindowsUpdateNotifier", "WindowsUpdateNotifier.config");

            if (_TryLoadConfigurationFile(configFile, out config))
                return config;

            // fallback - recreate the config file
            if (File.Exists(configFile))
                File.Delete(configFile);
            
            _TryLoadConfigurationFile(configFile, out config);
            
            return config;
        }

        private bool _TryLoadConfigurationFile(string configFile, out Configuration config)
        {
            config = null;

            try
            {
                var c = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
                config = ConfigurationManager.OpenMappedExeConfiguration(c, ConfigurationUserLevel.None);
                
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("The file {0} is not a valid configuration file!", configFile);
                return false;
            }
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

            if (mConfig.AppSettings.Contains(KB_IDS_TO_IGNORE) == false)
                mConfig.AppSettings.Settings.Add(KB_IDS_TO_IGNORE, WINDOWS_10_UPGRADE_KB_ID);
        }

        private void _CorrectKbIdsIfNeeded()
        {
            var kbIds = mConfig.AppSettings.Settings[KB_IDS_TO_INSTALL].Value;

            if (kbIds == WINDOWS_8_DEFENDER_KB_ID && UacHelper.IsRunningOnWindows7())
                _SetSetting(KB_IDS_TO_INSTALL, WINDOWS_7_DEFENDER_KB_ID);
        }

        private string _GetWindowsDefenderKbId()
        {
            return UacHelper.IsRunningOnWindows7()
                ? WINDOWS_7_DEFENDER_KB_ID
                : WINDOWS_8_DEFENDER_KB_ID; // use windows 8 kb-id as a default value even if it is not a windows 8 os.
        }

        private static string[] _ParseKbIds(string kbIds)
        {
            return kbIds.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
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