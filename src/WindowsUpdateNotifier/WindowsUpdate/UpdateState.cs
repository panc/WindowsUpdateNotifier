using System.Drawing;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public enum UpdateState
    {
        Searching,
        UpdatesAvailable,
        NoUpdatesAvailable
    }

    public static class UpdateStateExtensions
    {
        public static UpdateState GetUpdateState(this int updateCount)
        {
            return updateCount > 0
                ? UpdateState.UpdatesAvailable
                : UpdateState.NoUpdatesAvailable;
        }

        public static Icon GetIcon(this UpdateState state)
        {
            switch (state)
            {
                case UpdateState.Searching:
                    return ImageResources.WindowsUpdateSearching1;
                case UpdateState.UpdatesAvailable:
                    return ImageResources.WindowsUpdate;
                default:
                    return ImageResources.WindowsUpdateNo;
            }
        }
    }
}