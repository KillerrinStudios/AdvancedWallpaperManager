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
using System.Diagnostics;
using WallpaperManager.Services;
using KillerrinStudiosToolkit;

namespace WallpaperManager.BackgroundTasks
{
    public sealed class FileDiscoveryBackgroundTask : IBackgroundTask
    {
        public static bool IsTaskAllowedToRun()
        {
            // Check to make sure the Task is enabled in the settings menu
            var fileDiscoveryEnabled = new FileDiscoveryEnableSetting();
            if (!fileDiscoveryEnabled.Value) return false;

            // Due to Background Tasks limitations, ensure the value be >= 15 Minutes
            if (!DoesFrequencyMeetMinimumTimespan())
                return false;

            return true;
        }

        public static bool DoesFrequencyMeetMinimumTimespan()
        {
            var fileDiscoveryFrequency = new FileDiscoveryFrequencySetting();
            if (fileDiscoveryFrequency.Value < TimeSpan.FromMinutes(15))
                return false;
            return true;
        }

        BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(Run)} - Begun");

            // Register the Monitoring Events
            taskInstance.Canceled += TaskInstance_Canceled;
            taskInstance.Task.Completed += Task_Completed;
            taskInstance.Task.Progress += Task_Progress;

            // If the task is not allowed to run, return early
            if (!IsTaskAllowedToRun()) return;

            // Otherwise, we jump into File Discovery Process
            _deferral = taskInstance.GetDeferral();
            using (var context = new WallpaperManagerContext())
            {
                var themeRepo = new WallpaperThemeRepository(context);
                var fileDiscoveryService = new FileDiscoveryService(context);

                // Preform File Discovery for ALL of the themes
                //_deferral = taskInstance.GetDeferral();
                //await fileDiscoveryService.PreformFileDiscoveryAll(null);
                //_deferral.Complete();

                // Preform File Discovery for the Desktop Wallpaper Theme
                Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(ActiveDesktopThemeSetting)} - Started");
                var activeDesktopThemeSetting = new ActiveDesktopThemeSetting();
                var activeDesktopThemeSettingValue = activeDesktopThemeSetting.Value;
                if (activeDesktopThemeSettingValue.HasValue)
                {
                    var activeWallpaperTheme = themeRepo.Find(activeDesktopThemeSettingValue.Value);
                    await fileDiscoveryService.PreformFileDiscovery(activeWallpaperTheme, null);
                }
                Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(ActiveDesktopThemeSetting)} - Completed");

                // Preform File Discovery for the Lockscreen Theme
                Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(ActiveLockscreenThemeSetting)} - Started");
                var activeLockscreenThemeSetting = new ActiveLockscreenThemeSetting();
                var activeLockscreenThemeSettingValue = activeLockscreenThemeSetting.Value;
                if (activeLockscreenThemeSettingValue.HasValue &&
                    activeLockscreenThemeSettingValue != activeDesktopThemeSettingValue)
                {
                    var activeLockscreenTheme = themeRepo.Find(activeLockscreenThemeSettingValue.Value);
                    await fileDiscoveryService.PreformFileDiscovery(activeLockscreenTheme, null);
                }

                Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(ActiveLockscreenThemeSetting)} - Completed");
            }

            // Update the FileDiscovery Last Run
            var fileDiscoveryLastRunSetting = new FileDiscoveryLastRunSetting();
            fileDiscoveryLastRunSetting.Value = DateTime.UtcNow;

            Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(Run)} - Completed");
            _deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(TaskInstance_Canceled)}");
        }

        private void Task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(Task_Progress)} - {args.InstanceId} : {args.Progress}");
        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine($"{nameof(FileDiscoveryBackgroundTask)} - {nameof(Task_Completed)}");
        }
    }
}
