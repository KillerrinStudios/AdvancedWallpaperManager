using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models.Settings;
using Windows.ApplicationModel.Background;

namespace WallpaperManager.BackgroundTasks
{
    public class FileDiscoveryBackgroundTaskHelper
    {
        public const string TaskName = "FileDiscovery";
        public const string TaskEntryPoint = nameof(WallpaperManager) + "." + nameof(BackgroundTasks) + "." + nameof(FileDiscoveryBackgroundTask);

        public async Task<BackgroundTaskRegistration> RegisterBackgroundTask()
        {
            BackgroundTaskTools backgroundTaskTools = new BackgroundTaskTools();

            // Check if the Task already exists
            if (backgroundTaskTools.IsTaskRegistered(TaskName)) return null;

            // Check to make sure the Task is enabled in the settings menu
            if (!FileDiscoveryBackgroundTask.IsTaskAllowedToRun()) return null;

            // Ensure the app has permission to run the task
            var access = await backgroundTaskTools.RequestAccess();
            if (!access) return null;

            // Register the Task
            var builder = new BackgroundTaskBuilder();
            builder.Name = TaskName;
            builder.TaskEntryPoint = TaskEntryPoint;

            var fileDiscoveryFrequency = new FileDiscoveryFrequencySetting();
            builder.SetTrigger(new TimeTrigger((uint)fileDiscoveryFrequency.Value.TotalMinutes, false));

            BackgroundTaskRegistration task = builder.Register();
            return task;
        }
    }
}
