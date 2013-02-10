using System;
using System.Threading;

namespace WindowsUpdateNotifier
{
    public class SingleInstanceHelper
    {
        public static void MakeSingleInstance(string name, ISingleInstance app)
        {
            EventWaitHandle eventWaitHandle = null;
            var isFirstInstance = false;

            var eventName = Environment.MachineName + "-" + name;

            try
            {
                eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
            }
            catch
            {
                // it's the first instance
                isFirstInstance = true;
            }

            if (isFirstInstance)
            {
                eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);
                ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, _OnWaitOrTimerCallback, app, Timeout.Infinite, false);

                // not need anymore
                eventWaitHandle.Close();
            }
            else
            {
                // inform the first instance, that a new instanc has tried to start.
                eventWaitHandle.Set();
                Environment.Exit(0);
            }
        }

        private static void _OnWaitOrTimerCallback(object state, bool timedOut)
        {
            var app = (ISingleInstance)state;
            app.OnNewInstanceStarted();
        }
    }
}