using System;
using System.Collections.Generic;

namespace WindowsUpdateNotifier
{
    public class VersionHelper
    {
        private readonly List<Action<string>> mCallbacks;

        public VersionHelper()
        {
            mCallbacks = new List<Action<string>>();
        }

        public void RegisterForNotification(Action<string> callback)
        {
            if (mCallbacks.Contains(callback) == false)
                mCallbacks.Add(callback);
        }

        public void SearchForNewVersion()
        {
            
        }
    }
}