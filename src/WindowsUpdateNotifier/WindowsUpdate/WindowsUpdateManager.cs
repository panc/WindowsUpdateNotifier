using System;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsUpdateNotifier
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
                try
                {
                    var session = new UpdateSession();
                    var searcher = session.CreateUpdateSearcher();
                    searcher.Online = true;
                    var result = searcher.Search("IsInstalled=0");

                    return result.Updates.Count;
                }
                catch
                {
                    return -1;
                }
            })
            .ContinueWith(task => mOnSearchFinished(task.Result), scheduler);
        }
    }
}