using System.Windows;

namespace WindowsUpdateNotifier.Desktop
{
    public partial class App : Application
    {
        private ApplicationHandler mApplicationHandler;

        protected override void OnStartup(StartupEventArgs e)
        {
            mApplicationHandler = new ApplicationHandler();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mApplicationHandler.Dispose();
        }
    }
}
