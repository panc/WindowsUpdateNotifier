using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindowsUpdateNotifier.Versioning;

namespace WindowsUpdateNotifier
{
    public class VersionHelper
    {
        public VersionHelper()
        {
            var info = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            CurrentVersion = info.ProductVersion;
            Copyright = info.LegalCopyright;
        }

        public void SearchForNewVersion(Action onFinishedCallback)
        {
            var scheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var rdr = new RssVersionReader();
                var items = rdr.Execute();

                LatestVersion = items.OrderByDescending(x => x.Version).FirstOrDefault();
                
                IsNewVersionAvailable = 
                    LatestVersion != null && 
                    string.Compare(LatestVersion.Version, CurrentVersion, StringComparison.Ordinal) > 0;
            })
            .ContinueWith(x => onFinishedCallback(), scheduler);
        }

        public string Copyright { get; private set; }
        
        public string CurrentVersion { get; private set; }

        public RssVersionItem LatestVersion { get; private set; }

        public bool IsNewVersionAvailable { get; private set; }
    }
}