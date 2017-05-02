using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace KillerrinStudiosToolkit.BackgroundTasks
{
    public abstract class BackgroundTaskHelperBase
    {
        public BackgroundTaskTools m_backgroundTaskTools { get; protected set; } = new BackgroundTaskTools();
        public string TaskName { get; protected set; }
        public string TaskEntryPoint { get; protected set; }


        public BackgroundTaskHelperBase(string taskName, string taskEntryPoint) {
            TaskName = taskName;
            TaskEntryPoint = taskEntryPoint;
        }

        public async Task<BackgroundTaskRegistration> RegisterBackgroundTask()
        {
            // Allow the deriving class to determine whether or not to cancel the registration
            if (CancelTaskRegistration()) return null;

            // Check if the Task already exists and unregister it if it does
            UnregisterTask();

            // Ensure the app has permission to run the task
            var access = await m_backgroundTaskTools.RequestAccess();
            if (!access) return null;

            // Build the Task...
            var builder = BuildTask();

            // ...Then Register It
            BackgroundTaskRegistration task = builder.Register();
            return task;
        }

        protected abstract BackgroundTaskBuilder BuildTask();
        protected abstract bool CancelTaskRegistration();

        public bool IsTaskRegistered { get { return m_backgroundTaskTools.IsTaskRegistered(TaskName); } }
        public IBackgroundTaskRegistration GetTask { get { return m_backgroundTaskTools.GetRegisteredTask(TaskName); } }

        public void UnregisterTask()
        {
            var existingTask = m_backgroundTaskTools.GetRegisteredTask(TaskName);
            if (existingTask != null)
                existingTask.Unregister(false);
        }
    }
}
