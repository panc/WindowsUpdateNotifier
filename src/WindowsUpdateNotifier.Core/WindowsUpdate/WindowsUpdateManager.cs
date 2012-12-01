using System;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsUpdateNotifier.Core
{
    public class WindowsUpdateManager
    {
        private readonly Action<int> mOnSearchFinished;

        public WindowsUpdateManager(Action<int> onSearchFinished)
        {
            mOnSearchFinished = onSearchFinished;
        }

        public void StartSearchForUpdates()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                var session = new UpdateSession();
                var searcher = session.CreateUpdateSearcher();
                searcher.Online = true;
                var result = searcher.Search("IsInstalled=0");

                return result.Updates.Count;
            })
            .ContinueWith(task => mOnSearchFinished(task.Result), scheduler);
        }
    }
}