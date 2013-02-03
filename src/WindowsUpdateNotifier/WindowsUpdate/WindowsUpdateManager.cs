using System;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsUpdateNotifier
{
    public class WindowsUpdateManager
    {
        private readonly Action<UpdateResult> mOnSearchFinished;

        public WindowsUpdateManager(Action<UpdateResult> onSearchFinished)
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
                    var result = searcher.Search("IsInstalled=0 AND IsHidden=0");

                    var installedUpdates = _InstallUpdates(session, result.Updates);

                    return new UpdateResult(result.Updates.Count, installedUpdates);
                }
                catch
                {
                    return new UpdateResult();
                }
            })
            .ContinueWith(task => mOnSearchFinished(task.Result), scheduler);
        }

        private int _InstallUpdates(UpdateSession session, UpdateCollection updates)
        {
            var downloader = session.CreateUpdateDownloader();
            downloader.Updates = updates;
            downloader.Download();

            var updatesToInstall = new UpdateCollection();
            foreach (IUpdate update in updates)
            {
                if (update.IsDownloaded) updatesToInstall.Add(update);
            }

            var installer = session.CreateUpdateInstaller();
            installer.Updates = updatesToInstall;

            var installationRes = installer.Install();
            for (int i = 0; i < updatesToInstall.Count; i++)
            {
                if (installationRes.GetUpdateResult(i).HResult == 0)
                {
                    Console.WriteLine("Installed : " + updatesToInstall[i].Title);
                }
                else
                {
                    Console.WriteLine("Failed : " + updatesToInstall[i].Title);
                }
            }

            return -1;
        }
    }
}