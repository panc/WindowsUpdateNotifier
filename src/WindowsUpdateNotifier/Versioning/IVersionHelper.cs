using System;

namespace WindowsUpdateNotifier
{
    public interface IVersionHelper
    {
        void SearchForNewVersion(Action onFinishedCallback);
        
        string Copyright { get; }
        
        string CurrentVersion { get; }
        
        RssVersionItem LatestVersion { get; }
        
        bool IsNewVersionAvailable { get; }
    }
}