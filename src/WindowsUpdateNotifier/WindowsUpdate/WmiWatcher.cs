using System;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsUpdateNotifier
{
    public class WmiWatcher
    {
        private readonly Action mOnStateChanged;
        private bool mStopRequested;
        private bool mIsRunning;
        private ManagementEventWatcher mWatcher;

        public WmiWatcher(Action onStateChanged)
        {
            mOnStateChanged = onStateChanged;
        }

        public void Start()
        {
            if (mIsRunning && mStopRequested == false)
                return;

            mStopRequested = false;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    mWatcher = _CreateEventWatcher();
                    ManagementBaseObject result = null;

                    while (mStopRequested == false)
                    {
                        try
                        {
                            result = mWatcher.WaitForNextEvent();
                            break;
                        }
                        catch (ManagementException)
                        {
                            // timeout of WaitForNextEvent()
                            // -> wait again
                        }
                    }

                    mWatcher.Stop();
                    mIsRunning = false;

                    if (result != null && mStopRequested == false)
                        UiThreadHelper.BeginInvokeInUiThread(mOnStateChanged);
                }
                finally
                {
                    if (mWatcher != null)
                        mWatcher.Dispose();
                }

            }, CancellationToken.None, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);
        }

        public void Stop()
        {
            mStopRequested = true;

            if (mWatcher != null)
            {
                mWatcher.Stop();
                mWatcher.Dispose();
            }
        }

        private ManagementEventWatcher _CreateEventWatcher()
        {
            // create event query to be notified within 1 second of a change in a service
            var query = new WqlEventQuery(
                "__InstanceDeletionEvent",
                new TimeSpan(0, 0, 1),
                "TargetInstance isa \"Win32_Process\" And TargetInstance.Name=\"wuauclt.exe\"");

            return new ManagementEventWatcher
            {
                Query = query,
                Options =
                {
                    Timeout = new TimeSpan(0, 0, 5) // times out watcher.WaitForNextEvent in 5 seconds
                }
            };
        }
    }
}