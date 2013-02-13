using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using S = TaskScheduler;

namespace WindowsUpdateNotifier
{
    public class TaskSchedulerAdapter : IDisposable
    {
        private const string APP_NAME = "WindowsUpdateNotifierStartup";

        private S.ITaskService mService;
        private S.ITaskFolder mRootFolder;

        public TaskSchedulerAdapter()
        {
            mService = new S.TaskScheduler();
            mService.Connect();

            mRootFolder = mService.GetFolder(@"\");
        }

        public void Create()
        {
            var userId = WindowsIdentity.GetCurrent().Name;

            // create the task
            var definition = mService.NewTask(0);
            definition.Settings.Hidden = true;
            definition.Settings.Enabled = true;

            definition.Principal.RunLevel = S._TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            definition.Principal.UserId = userId;
            
            definition.RegistrationInfo.Author = "Windows Update Notifier";
            definition.RegistrationInfo.Description = "Starts the Windows Update Notifier after starup";

            var trigger = (S.ILogonTrigger)definition.Triggers.Create(S._TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
            trigger.Id = APP_NAME;
            trigger.UserId = userId;

            var action = (S.IExecAction)definition.Actions.Create(S._TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = Assembly.GetExecutingAssembly().Location;

            mRootFolder.RegisterTaskDefinition(APP_NAME, definition, 
                6 /* createOrUpdateTask */, userId, null, S._TASK_LOGON_TYPE.TASK_LOGON_NONE);
        }

        public void DeleteTask()
        {
            var task = mRootFolder.GetTask(APP_NAME);

            if (task != null)
                mRootFolder.DeleteTask(APP_NAME, 0);
        }

        public bool CheckTaskExists()
        {
            return mRootFolder.GetTask(APP_NAME) != null;
        }

        public void Dispose()
        {
            if (mService != null)
            {
                Marshal.ReleaseComObject(mService);
                mService = null;
            }

            if (mRootFolder != null)
            {
                Marshal.ReleaseComObject(mRootFolder);
                mRootFolder = null;
            }
        }
    } 
} 