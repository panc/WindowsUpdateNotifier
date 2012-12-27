using System.Drawing;
using WindowsUpdateNotifier.Resources;

namespace WindowsUpdateNotifier
{
    public enum UpdateState
    {
        Searching,
        UpdatesAvailable,
        NoUpdatesAvailable,
        Failure
    }

    public static class UpdateStateExtensions
    {
        public static UpdateState GetUpdateState(this int updateCount)
        {
            return updateCount > 0
                ? UpdateState.UpdatesAvailable
                : updateCount == 0
                    ? UpdateState.NoUpdatesAvailable
                    : UpdateState.Failure;
        }

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