using System;
using System.ComponentModel;
using System.Diagnostics;
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
            var settings = AppSettings.Instance;
            RefreshInterval = settings.RefreshInterval;
            HideIcon = settings.HideIcon;
            UseMetroStyle = settings.UseMetroStyle;
            InstallUpdates = settings.InstallUpdates;
            CanInstallUpdates = UacHelper.IsRunningAsAdmin();

            IsSetAsAutoStartup = StartupShortcutHelper.IsSetAsAutoStartup();
            HelpLink = "http://wun.codeplex.com/";

            SaveAndCloseCommand = new SimpleCommand(() => _SaveAndClose(closeWindowCallback));
            ShowHelpCommand = new SimpleCommand(_ShowHelp);
        }

        public ICommand SaveAndCloseCommand { get; set; }

        public ICommand ShowHelpCommand { get; set; }

        public bool IsSetAsAutoStartup { get; set; }

        public bool HideIcon { get; set; }

        public bool UseMetroStyle { get; set; }

        public string HelpLink { get; set; }
        
        public bool InstallUpdates { get; set; }

        public bool CanInstallUpdates { get; set; }

        public int RefreshInterval
        {
            get { return mRefreshInterval; }
            set
            {
                mRefreshInterval = value;
                OnPropertyChanged("RefreshInterval");
            }
        }

        private void _SaveAndClose(Action close)
        {
            AppSettings.Instance.Save(RefreshInterval, HideIcon, UseMetroStyle, InstallUpdates);

            if (IsSetAsAutoStartup)
                StartupShortcutHelper.CreateStartupShortcut();
            else
                StartupShortcutHelper.DeleteStartupShortcut();

            close();
        }

        private void _ShowHelp()
        {
            Process.Start(HelpLink);
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