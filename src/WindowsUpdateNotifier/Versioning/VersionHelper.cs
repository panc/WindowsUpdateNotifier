using System;
using System.Linq;
using System.Threading.Tasks;
using WindowsUpdateNotifier.Versioning;

namespace WindowsUpdateNotifier
{
    public class VersionHelper
    {
        public void SearchForNewVersion(Action onFinishedCallback)
        {
            var scheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var rdr = new RssVersionReader();
                var items = rdr.Execute();

                LatestVersion = items.OrderBy(x => x.Date).FirstOrDefault(); // todo: date is not correct
                
                IsNewVersionAvailable = true;
            })
            .ContinueWith(x => onFinishedCallback(), scheduler);
        }

        public RssVersionItem LatestVersion { get; private set; }

        public bool IsNewVersionAvailable { get; private set; }
    }
}