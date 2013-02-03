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
        }

        public int AvailableUpdates { get; private set; }

        public int InstalledUpdates { get; private set; }
    }
}