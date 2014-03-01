using System;

namespace WindowsUpdateNotifier
{
    public class MockVersionHelper : IVersionHelper
    {
        public void SearchForNewVersion(Action onFinishedCallback)
        {
            onFinishedCallback();
        }

        public string Copyright { get; set; }

        public string CurrentVersion { get; set; }

        public bool IsNewVersionAvailable { get; set; }

        public RssVersionItem LatestVersion
        {
            get { return new RssVersionItem("Temp", "Temp", DateTime.Now); }
        }
    }
}