using System;
using System.Management;

namespace WindowsUpdateNotifier
{
    public class WmiWatcher
    {
        private ManagementEventWatcher mWatcher;
        //http://codelog.blogial.com/2009/06/25/managementeventwatcher-in-c/ 

        public void Start()
        {
//            var query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent");
//            
//            mWatcher = new ManagementEventWatcher(query);
//            mWatcher.EventArrived += _ProcessDeleted;
//            mWatcher.Start();

            // Create event query to be notified within 1 second of  
            // a change in a service
            WqlEventQuery query =
                new WqlEventQuery("__InstanceDeletionEvent",
                new TimeSpan(0, 0, 1),
                "TargetInstance isa \"Win32_Process\"");

            // Initialize an event watcher and subscribe to events  
            // that match this query
            var watcher = new ManagementEventWatcher();
            watcher.Query = query;
            // times out watcher.WaitForNextEvent in 5 seconds
            watcher.Options.Timeout = new TimeSpan(0, 0, 5);

            // http://msdn.microsoft.com/en-us/library/dd537607.aspx
            // http://msdn.microsoft.com/en-us/library/system.management.managementeventwatcher.aspx
            // Block until the next event occurs  
            // Note: this can be done in a loop if waiting for  
            //        more than one occurrence
            Console.WriteLine(
                "Open an application (notepad.exe) to trigger an event.");
            ManagementBaseObject e = watcher.WaitForNextEvent();

            //Display information from the event
            Console.WriteLine(
                "Process {0} has been created, path is: {1}",
                ((ManagementBaseObject)e["TargetInstance"])["Name"],
                ((ManagementBaseObject)e["TargetInstance"])["ExecutablePath"]);

            //Cancel the subscription
            watcher.Stop();
        }

        public void Stop()
        {
            if(mWatcher != null)
            {
                mWatcher.Stop();
                mWatcher.Dispose();
            }
        }

        private void _ProcessDeleted(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine(e.NewEvent.Properties["processname"].Value);

            if(e.NewEvent.Properties["processname"].Value == "wuauclt.exe")
            {
                // todo
            }
        }
    }
}