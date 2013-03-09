//http://codelog.blogial.com/2009/06/25/managementeventwatcher-in-c/ 
// http://msdn.microsoft.com/en-us/library/dd537607.aspx
// http://msdn.microsoft.com/en-us/library/system.management.managementeventwatcher.aspx

using System;
using System.Management;
using System.Threading.Tasks;

namespace WindowsUpdateNotifier
{
    public class WmiWatcher
    {
        private readonly Action mOnStateChanged;
        private bool mStopRequested;
        private bool mIsRunning;

        public WmiWatcher(Action onStateChanged)
        {
            mOnStateChanged = onStateChanged;
        }

        //                        Console.WriteLine(
        //                            "Process {0} has been created, path is: {1}",
        //                            ((ManagementBaseObject) result["TargetInstance"])["Name"],
        //                            ((ManagementBaseObject) result["TargetInstance"])["ExecutablePath"]);


        public void Start()
        {
            if(mIsRunning)
                return;

            Task.Factory.StartNew(() =>
            {
                using (var watcher = _CreateEventWatcher())
                {
                    ManagementBaseObject result = null;

                    while (mStopRequested == false)
                    {
                        try
                        {
                            result = watcher.WaitForNextEvent();
                            break;
                        }
                        catch (ManagementException)
                        {
                            // timeout in WaitForNextEvent
                            // -> wait again
                        }
                    }

                    watcher.Stop();
                    mIsRunning = false;

                    if (result != null && mStopRequested == false)
                        UiThreadHelper.BeginInvokeInUiThread(mOnStateChanged);
                }
            });
        }

        public void Stop()
        {
            mStopRequested = true;
        }

        private ManagementEventWatcher _CreateEventWatcher()
        {
            // create event query to be notified within 1 second of a change in a service
            var query = new WqlEventQuery(
                "__InstanceDeletionEvent",
                new TimeSpan(0, 0, 1),
                "TargetInstance isa \"Win32_Process\"");

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