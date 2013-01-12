using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public class SettingsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private int mRefreshInterval;
        private string mStartupShortcutInfo;

        public SettingsViewModel(Action closeWindowCallback)
        {
            var settings = AppSettings.Instance;
            RefreshInterval = settings.RefreshInterval;
            HideIcon = settings.HideIcon;
            StartupShortcutInfo = TextResources.Label_CanCreateStartupShortcut;

            SaveAndCloseCommand = new SimpleCommand(() => _SaveAndClose(closeWindowCallback));
            CreateStartupShortcutCommand = new SimpleCommand(_CreateStartupShortcut, _CanCreateStartupShortcut);
        }

        public ICommand SaveAndCloseCommand { get; set; }

        public SimpleCommand CreateStartupShortcutCommand { get; set; }

        public bool HideIcon { get; set; }

        public int RefreshInterval
        {
            get { return mRefreshInterval; }
            set
            {
                mRefreshInterval = value;
                OnPropertyChanged("RefreshInterval");
            }
        }

        public string StartupShortcutInfo
        {
            get { return mStartupShortcutInfo; }
            set
            {
                mStartupShortcutInfo = value;
                OnPropertyChanged("StartupShortcutInfo");
            }
        }

        private void _SaveAndClose(Action close)
        {
            AppSettings.Instance.Save(RefreshInterval, HideIcon);
            close();
        }

        private bool _CanCreateStartupShortcut()
        {
            var canCreate = _CheckForShotcut() == false;

            StartupShortcutInfo = canCreate
                ? TextResources.Label_CanCreateStartupShortcut
                : TextResources.Label_CanNotCreateStartupShortcut;

            return canCreate;
        }

        private void _CreateStartupShortcut()
        {
            var link = new ShellLink { Target = Assembly.GetExecutingAssembly().Location };

            link.Save(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup), 
                Path.GetFileNameWithoutExtension(link.Target) + ".lnk"));

            CreateStartupShortcutCommand.OnCanExecuteChanged();
        }

        private bool _CheckForShotcut()
        {
            var exePath = Assembly.GetExecutingAssembly().Location;

            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
            {
                if (file.EndsWith("lnk") == false)
                    continue;

                var link = new ShellLink(file);
                if (link.Target == exePath)
                    return true;
            }

            return false;
        }

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
    }
}