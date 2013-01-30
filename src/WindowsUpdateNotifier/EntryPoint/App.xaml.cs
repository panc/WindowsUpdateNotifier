using System;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class App : ISingleInstance
    {
        private ApplicationHandler mApplicationHandler;

        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstanceHelper.MakeSingleInstance("WindowsUpdateNotifier", this);
            mApplicationHandler = new ApplicationHandler();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mApplicationHandler.Dispose();
        }

        public void OnNewInstanceStarted()
        {
            Dispatcher.BeginInvoke((Action)(() => mApplicationHandler.OpenSettings()), null);            
        }
    }
}
