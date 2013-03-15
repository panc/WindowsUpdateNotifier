using System;
using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class App : ISingleInstance
    {
        private ApplicationRuntime mApplicationRuntime;

        protected override void OnStartup(StartupEventArgs e)
        {
            // ensure that only a single instance of the application is started
            // the app is clossed if another instance is already running
            SingleInstanceHelper.MakeSingleInstance("WindowsUpdateNotifier", this);

            var cmdHelper = new CommandLineHelper();

            AppSettings.Initialize(cmdHelper.UseDefaultSettings);
            mApplicationRuntime = new ApplicationRuntime(cmdHelper.CloseAfterCheck);
            mApplicationRuntime.Start();    
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mApplicationRuntime.Dispose();
        }

        public void OnNewInstanceStarted()
        {
            Dispatcher.BeginInvoke((Action)(() => mApplicationRuntime.OpenSettings()), null);            
        }
    }
}
