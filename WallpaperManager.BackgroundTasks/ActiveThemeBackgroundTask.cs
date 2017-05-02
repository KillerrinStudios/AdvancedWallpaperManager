using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using WallpaperManager.DAL.Repositories;
using WallpaperManager.Services;
using KillerrinStudiosToolkit;

namespace WallpaperManager.BackgroundTasks
{
    public sealed class ActiveThemeBackgroundTask : IBackgroundTask
    {
        public static bool IsTaskAllowedToRun()
        {
            return true;
        }

        BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - {nameof(Run)} - Begun");

            // Register the Monitoring Events
            taskInstance.Canceled += TaskInstance_Canceled;
            taskInstance.Task.Completed += Task_Completed;
            taskInstance.Task.Progress += Task_Progress;

            // Create the deferal and jump into the Task
            _deferral = taskInstance.GetDeferral();
            using (var context = new WallpaperManagerContext())
            {
                var themeRepo = new WallpaperThemeRepository(context);
                var activeThemeService = new ActiveThemeService();

                // Preform the service for the Active Desktop Theme
                Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - Active Desktop - Started");
                await activeThemeService.NextDesktopBackground();
                Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - Active Desktop - Completed");

                // Preform the service for the Active Lockscreen Theme
                Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - Active Lockscreen - Started");
                await activeThemeService.NextLockscreenBackground();
                Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - Active Lockscreen - Completed");
            }

            // Trigger the task is compelted
            Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - {nameof(Run)} - Completed");
            _deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - {nameof(TaskInstance_Canceled)}");
        }

        private void Task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - {nameof(Task_Progress)} - {args.InstanceId} : {args.Progress}");
        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine($"{nameof(ActiveThemeBackgroundTask)} - {nameof(Task_Completed)}");
        }
    }
}
