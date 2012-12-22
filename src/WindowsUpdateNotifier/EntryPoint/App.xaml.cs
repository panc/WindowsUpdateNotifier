using System.Windows;

namespace WindowsUpdateNotifier
{
    public partial class App
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
