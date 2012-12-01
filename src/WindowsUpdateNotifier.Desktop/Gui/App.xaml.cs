using System.Windows;

namespace WindowsUpdateNotifier.Desktop
{
    public partial class App : Application
    {
        private MainThread mMainThread;

        protected override void OnStartup(StartupEventArgs e)
        {
            mMainThread = new MainThread();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mMainThread.Dispose();
        }
    }
}
