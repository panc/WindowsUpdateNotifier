using System;
using System.Linq;
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

        public void StartSearchForUpdates(string[] kbIdsToInstall, string[] kbIdsToIgnore)
        {
            var scheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var session = new UpdateSession();
                    var searcher = session.CreateUpdateSearcher();
                    searcher.Online = true;
                    var result = searcher.Search("IsInstalled=0 AND IsHidden=0");
                    
                    var installedUpdates = 0;
                    if (kbIdsToInstall.Length > 0)
                    {
                        var updatesToInstall = _GetUpdatesToInstall(kbIdsToInstall, result.Updates);
                        installedUpdates = _InstallUpdates(session, updatesToInstall);
                    }

                    var updatesToIgnore = 0;
                    if (kbIdsToIgnore.Length > 0)
                    {
                        updatesToIgnore = _GetUpdatesToIgnore(result.Updates, kbIdsToIgnore);
                    }

                    return new UpdateResult(result.Updates.Count - installedUpdates - updatesToIgnore, installedUpdates);
                }
                catch (Exception)
                {
                    return new UpdateResult();
                }
            })
            .ContinueWith(task => mOnSearchFinished(task.Result), scheduler);
        }

        private int _GetUpdatesToIgnore(UpdateCollection updates, string[] kbIdsToIgnore)
        {
            var count = 0;
            foreach (IUpdate update in updates)
            {
                foreach (var id in update.KBArticleIDs)
                {
                    if (kbIdsToIgnore.Any(x => Equals(id, x)))
                        count++;
                }
            }

            return count;
        }

        private UpdateCollection _GetUpdatesToInstall(string[] kbIdsToInstall, UpdateCollection updates)
        {
            var updatesToInstall = new UpdateCollection();

            foreach (IUpdate update in updates)
            {
                foreach (var id in update.KBArticleIDs)
                {
                    if (kbIdsToInstall.Any(x => Equals(id, x)))
                        updatesToInstall.Add(update);
                }
            }

            return updatesToInstall;
        }

        private int _InstallUpdates(UpdateSession session, UpdateCollection updates)
        {
            if (updates.Count < 1 || UacHelper.IsRunningAsAdmin() == false)
                return 0;

            try
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

                var result = installer.Install();
                var numberOfInstalledUpdates = 0;

                for (var i = 0; i < updatesToInstall.Count; i++)
                {
                    if (result.GetUpdateResult(i).HResult == 0)
                        numberOfInstalledUpdates++;
                }

                return numberOfInstalledUpdates;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}