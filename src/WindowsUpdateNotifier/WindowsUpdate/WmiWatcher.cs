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
            var query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent");
            
            mWatcher = new ManagementEventWatcher(query);
            mWatcher.EventArrived += _ProcessDeleted;
            mWatcher.Start();
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