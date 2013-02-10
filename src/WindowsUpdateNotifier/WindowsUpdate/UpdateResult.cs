namespace WindowsUpdateNotifier
{
    public class UpdateResult
    {
        public UpdateResult()
            : this(-1, -1)
        {
        }

        public UpdateResult(int availableUpdates)
            : this(availableUpdates, -1)
        {
        }

        public UpdateResult(int availableUpdates, int installedUpdates)
        {
            AvailableUpdates = availableUpdates;
            InstalledUpdates = installedUpdates;
            UpdateState = _GetUpdateState();
        }

        public int AvailableUpdates { get; private set; }

        public int InstalledUpdates { get; private set; }

        public UpdateState UpdateState { get; private set; }

        private UpdateState _GetUpdateState()
        {
            // UpdateAvailable doesn't give any information about how many updates where installed
            // but we ignore this case an only show the information that update are available

            if (AvailableUpdates > 0)
                return UpdateState.UpdatesAvailable;

            if (InstalledUpdates > 0)
                return UpdateState.UpdatesInstalled;

            if (AvailableUpdates < 0)
                return UpdateState.Failure;

            return UpdateState.NoUpdatesAvailable;
        }
    }
}