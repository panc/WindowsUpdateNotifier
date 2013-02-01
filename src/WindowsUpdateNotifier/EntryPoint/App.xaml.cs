using System;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class App : ISingleInstance
    {
        private ApplicationHandler mApplicationHandler;

        protected override void OnStartup(StartupEventArgs e)
        {
            // ensure that only a single instance of the application is started
            // the app is clossed if another instance is already running
            SingleInstanceHelper.MakeSingleInstance("WindowsUpdateNotifier", this);

            var cmdHelper = new CommandLineHelper();

            AppSettings.Initialize(cmdHelper.UseDefaultSettings);
            mApplicationHandler = new ApplicationHandler(cmdHelper.CloseAfterCheck);
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
