using System;
using System.ComponentModel;
using System.Windows.Input;

namespace WindowsUpdateNotifier
{
    public class SettingsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private int mRefreshInterval;

        public SettingsViewModel(Action closeWindowCallback)
        {
            var settings = AppSettings.Instance;
            RefreshInterval = settings.RefreshInterval;
            HideIcon = settings.HideIcon;

            SaveAndCloseCommand = new SimpleCommand(() => _SaveAndClose(closeWindowCallback));
        }

        public ICommand SaveAndCloseCommand { get; set; }

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

        private void _SaveAndClose(Action close)
        {
            AppSettings.Instance.Save(RefreshInterval, HideIcon);
            close();
        }

        public string this[string columnName]
        {
            get { return columnName == "RefreshInterval" ? Error : string.Empty; }
        }

        public string Error { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}