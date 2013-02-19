using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using S = TaskScheduler;

namespace WindowsUpdateNotifier
{
    public class TaskSchedulerWrapper : IDisposable
    {
        private const string APP_NAME = "WindowsUpdateNotifierStartup";

        private S.ITaskService mService;
        private S.ITaskFolder mRootFolder;

        public TaskSchedulerWrapper()
        {
            mService = new S.TaskScheduler();
            mService.Connect();

            mRootFolder = mService.GetFolder(@"\");
        }

        public void CreateTask()
        {
            var userId = WindowsIdentity.GetCurrent().Name;

            var taskDefinition = mService.NewTask(0);
            taskDefinition.Settings.Hidden = true;
            taskDefinition.Settings.Enabled = true;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            taskDefinition.Settings.MultipleInstances = S._TASK_INSTANCES_POLICY.TASK_INSTANCES_IGNORE_NEW;
            taskDefinition.Settings.RunOnlyIfIdle = false;
            taskDefinition.Settings.AllowHardTerminate = false;
            taskDefinition.Settings.ExecutionTimeLimit = "PT0S";

            taskDefinition.Principal.RunLevel = S._TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            taskDefinition.Principal.UserId = userId;
            
            taskDefinition.RegistrationInfo.Author = "Windows Update Notifier";
            taskDefinition.RegistrationInfo.Description = "Starts the Windows Update Notifier after starup";

            var trigger = (S.ILogonTrigger)taskDefinition.Triggers.Create(S._TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
            trigger.Id = APP_NAME;
            trigger.UserId = userId;

            var action = (S.IExecAction)taskDefinition.Actions.Create(S._TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = Assembly.GetExecutingAssembly().Location;

            mRootFolder.RegisterTaskDefinition(APP_NAME, taskDefinition, 
                6 /* createOrUpdateTask */, userId, null, S._TASK_LOGON_TYPE.TASK_LOGON_NONE);
        }

        public void DeleteTaskIfNeeded()
        {
            if (CheckTaskExists())
                mRootFolder.DeleteTask(APP_NAME, 0);
        }

        public bool CheckTaskExists()
        {
            return mRootFolder.GetTasks(1).Cast<S.IRegisteredTask>().Any(task => task.Name == APP_NAME);
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