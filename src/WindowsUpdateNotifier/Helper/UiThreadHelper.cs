using System;
using System.Windows.Threading;

namespace WindowsUpdateNotifier
{
    public static class UiThreadHelper
    {
        private static Action<Action> sExecutor;

        public static void InitializeWithCurrentDispatcher()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            sExecutor = action =>
            {
                if (dispatcher.CheckAccess())
                    action();
                else
                    dispatcher.BeginInvoke(action);
            };
        }

        public static void BeginInvokeInUiThread(Action action)
        {
            sExecutor(action);
        }
    }
}