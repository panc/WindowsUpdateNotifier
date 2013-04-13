namespace WindowsUpdateNotifier
{
    public static class StartupHelper
    {
        public static void UpdateStartupSettings(bool isSetAsAutoStartup)
        {
            if (isSetAsAutoStartup)
                _AddToAutostart();
            else
                _RemoveFromAutostart();
        }

        public static void UpdateExecPath()
        {
            using (var adapter = new TaskSchedulerWrapper())
            {
                if (adapter.CheckExecPathIsCorrect())
                    return;

                if (adapter.CheckTaskExists())
                {
                    if (UacHelper.IsRunningAsAdmin() == false)
                        // we can do nothing if the app is not started in admin mode
                        return;

                    adapter.DeleteTaskIfNeeded();
                    adapter.CreateTask();
                }
                else
                {
                    ShortcutHelper.CreateStartupShortcut();
                }
            }
        }

        public static bool IsSetAsAutoStartup()
        {
            using (var adapter = new TaskSchedulerWrapper())
            {
                if (adapter.CheckExecPathIsCorrect())
                    return true;
            }

            return ShortcutHelper.IsSetAsAutoStartup();
        }

        private static void _AddToAutostart()
        {
            using (var adapter = new TaskSchedulerWrapper())
            {
                if (adapter.CheckExecPathIsCorrect())
                {
                    // There is nothing to do if a task is already created and the 
                    // execution path is correct even if admin rights are not 
                    // applied to the app at the moment.
                    return;
                }

                if (UacHelper.IsRunningAsAdmin())
                {
                    ShortcutHelper.DeleteStartupShortcut();
                    adapter.DeleteTaskIfNeeded();
                    adapter.CreateTask();
                }
                else
                {
                    ShortcutHelper.CreateStartupShortcut();
                }
            }
        }

        private static void _RemoveFromAutostart()
        {
            // remove shortcut if needed
            ShortcutHelper.DeleteStartupShortcut();

            // Try to remove the task from the task scheduler if needed.
            // This is only possible if admin rights are present. Otherwise an exception
            // will be thrown which is catched in the settingsview model.
            using (var adapter = new TaskSchedulerWrapper())
            {
                adapter.DeleteTaskIfNeeded();
            }
        }
    }
}