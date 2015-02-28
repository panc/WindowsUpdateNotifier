﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class SettingsViewModel : ViewModel, IDataErrorInfo
    {
        private int mRefreshInterval;
        private bool mSaveFailed;

        public SettingsViewModel()
        {
        }

        public SettingsViewModel(Action closeWindowCallback, VersionHelper versionHelper)
        {
            CanInstallUpdates = UacHelper.IsRunningAsAdmin();
            CanNotInstallUpdates = !CanInstallUpdates;

            var settings = AppSettings.Instance;
            RefreshInterval = settings.RefreshInterval;
            HideIcon = settings.HideIcon;
            DisableNotifications = settings.DisableNotifications;
            UseMetroStyle = settings.UseMetroStyle;
            InstallUpdates = settings.InstallUpdates && CanInstallUpdates;

            IsSetAsAutoStartup = StartupHelper.IsSetAsAutoStartup();
            HelpLink = "http://wun.codeplex.com/";
            HowToStartAsAdminLink = "http://wun.codeplex.com/wikipage?title=HowToStartAsAdmin";

            Version = string.Format("Version {0}  © Christoph Pangerl", versionHelper.CurrentVersion);
            AutoInstallComment = string.Format(TextResources.Label_AutoInstallComment, string.Join(", KB", settings.KbIdsToInstall));

            SaveAndCloseCommand = new SimpleCommand(() => _SaveAndClose(closeWindowCallback));
            ShowHelpCommand = new SimpleCommand(() => _OpenLink(HelpLink));
            ShowHowToStartAsAdminCommand = new SimpleCommand(() => _OpenLink(HowToStartAsAdminLink));
        }

        #region Properties

        public ICommand SaveAndCloseCommand { get; set; }

        public ICommand ShowHelpCommand { get; set; }

        public ICommand ShowHowToStartAsAdminCommand { get; set; }

        public bool IsSetAsAutoStartup { get; set; }

        public bool HideIcon { get; set; }
        
        public bool DisableNotifications { get; set; }

        public bool UseMetroStyle { get; set; }

        public string HelpLink { get; set; }

        public string Version { get; set; }

        public string HowToStartAsAdminLink { get; set; }

        public bool InstallUpdates { get; set; }

        public bool CanInstallUpdates { get; set; }

        public bool CanNotInstallUpdates { get; set; }

        public string AutoInstallComment { get; set; }

        public string AdditionalKbIds { get; set; }

        public int RefreshInterval
        {
            get { return mRefreshInterval; }
            set
            {
                mRefreshInterval = value;
                RaisePropertyChanged(() => RefreshInterval);
            }
        }

        public bool SaveFailed
        {
            get { return mSaveFailed; }
            set
            {
                mSaveFailed = value;
                RaisePropertyChanged(() => SaveFailed);
            }
        }
        
        #endregion

        private void _SaveAndClose(Action close)
        {
            try
            {
                SaveFailed = false;
                AppSettings.Instance.Save(RefreshInterval, HideIcon, DisableNotifications, UseMetroStyle, InstallUpdates, AdditionalKbIds);
                StartupHelper.UpdateStartupSettings(IsSetAsAutoStartup);

                close();
            }
            catch (Exception)
            {
                SaveFailed = true;
            }
        }

        private void _OpenLink(string link)
        {
            try
            {
                Process.Start(link);
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        #region IDataError interface

        public string this[string columnName]
        {
            get { return columnName == "RefreshInterval" ? Error : string.Empty; }
        }

        public string Error { get; set; }

        #endregion
    }
}