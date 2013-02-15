using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace WindowsUpdateNotifier
{
    public class SettingsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private int mRefreshInterval;

        public SettingsViewModel()
        {
        }

        public SettingsViewModel(Action closeWindowCallback)
        {
            CanInstallUpdates = UacHelper.IsRunningAsAdmin();
            CanNotInstallUpdates = !CanInstallUpdates;

            var settings = AppSettings.Instance;
            RefreshInterval = settings.RefreshInterval;
            HideIcon = settings.HideIcon;
            UseMetroStyle = settings.UseMetroStyle;
            InstallUpdates = settings.InstallUpdates && CanInstallUpdates;

            IsSetAsAutoStartup = StartupHelper.IsSetAsAutoStartup();
            HelpLink = "http://wun.codeplex.com/";
            HowToStartAsAdminLink = "http://wun.codeplex.com/wikipage?title=HowToStartAsAdmin";

            Version = string.Format("Version {0}  © Christoph Pangerl", _GetVersion());

            SaveAndCloseCommand = new SimpleCommand(() => _SaveAndClose(closeWindowCallback));
            ShowHelpCommand = new SimpleCommand(_ShowHelp);
            ShowHowToStartAsAdminCommand = new SimpleCommand(_ShowHowToStartAsAdmin);
        }

        #region Properties

        public ICommand SaveAndCloseCommand { get; set; }

        public ICommand ShowHelpCommand { get; set; }

        public ICommand ShowHowToStartAsAdminCommand { get; set; }

        public bool IsSetAsAutoStartup { get; set; }

        public bool HideIcon { get; set; }

        public bool UseMetroStyle { get; set; }

        public string HelpLink { get; set; }

        public string Version { get; set; }

        public string HowToStartAsAdminLink { get; set; }

        public bool InstallUpdates { get; set; }

        public bool CanInstallUpdates { get; set; }

        public bool CanNotInstallUpdates { get; set; }

        public int RefreshInterval
        {
            get { return mRefreshInterval; }
            set
            {
                mRefreshInterval = value;
                OnPropertyChanged("RefreshInterval");
            }
        }

        #endregion 

        private void _SaveAndClose(Action close)
        {
            AppSettings.Instance.Save(RefreshInterval, HideIcon, UseMetroStyle, InstallUpdates);
            StartupHelper.UpdateStartupSettings(IsSetAsAutoStartup);
            
            close();
        }

        private void _ShowHelp()
        {
            Process.Start(HelpLink);
        }

        private void _ShowHowToStartAsAdmin()
        {
            Process.Start(HowToStartAsAdminLink);
        }

        private string _GetVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        #region IDataError interface

        public string this[string columnName]
        {
            get { return columnName == "RefreshInterval" ? Error : string.Empty; }
        }

        public string Error { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}