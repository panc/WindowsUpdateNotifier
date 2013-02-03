using System.Drawing;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public enum UpdateState
    {
        Searching,
        UpdatesAvailable,
        NoUpdatesAvailable,
        UpdatesInstalled,
        Failure
    }

    public static class UpdateStateExtensions
    {
        public static Icon GetIcon(this UpdateState state)
        {
            switch (state)
            {
                case UpdateState.Searching:
                    return ImageResources.WindowsUpdateSearching1;
                case UpdateState.UpdatesAvailable:
                    return ImageResources.WindowsUpdate;
                case UpdateState.Failure:
                    return ImageResources.WindowsUpdateNoConnection;
                default:
                    return ImageResources.WindowsUpdateNothing;
            }
        }
    }
}