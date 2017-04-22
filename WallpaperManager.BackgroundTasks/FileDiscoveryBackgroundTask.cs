using System;
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
            var fileDiscoveryFrequency = new FileDiscoveryFrequencySetting();
            if (fileDiscoveryFrequency.Value.Days <= 0 &&
                fileDiscoveryFrequency.Value.Hours <= 0 &&
                fileDiscoveryFrequency.Value.Minutes < 15)
                return false;

            return true;
        }

        BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Register the Monitoring Events
            taskInstance.Canceled += TaskInstance_Canceled;
            taskInstance.Task.Completed += Task_Completed;
            taskInstance.Task.Progress += Task_Progress;

            // If the task is not allowed to run, return early
            if (!IsTaskAllowedToRun()) return;

            // Otherwise, we jump into File Discovery Process
            using (var context = new WallpaperManagerContext())
            {
                var themeRepo = new WallpaperThemeRepository(context);
                var fileDiscoveryService = new FileDiscoveryService(context);

                // Preform File Discovery for ALL of the themes
                //_deferral = taskInstance.GetDeferral();
                //await fileDiscoveryService.PreformFileDiscoveryAll(null);
                //_deferral.Complete();

                // Preform File Discovery for the Desktop Wallpaper Theme
                _deferral = taskInstance.GetDeferral();
                var activeDesktopThemeSetting = new ActiveDesktopThemeSetting();
                if (activeDesktopThemeSetting.Value.HasValue)
                {
                    var activeWallpaperTheme = themeRepo.Find(activeDesktopThemeSetting.Value.Value);
                    await fileDiscoveryService.PreformFileDiscovery(activeWallpaperTheme, null);
                }
                _deferral.Complete();

                // Preform File Discovery for the Lockscreen Theme
                _deferral = taskInstance.GetDeferral();
                var activeLockscreenThemeSetting = new ActiveLockscreenThemeSetting();
                if (activeLockscreenThemeSetting.Value.HasValue &&
                    activeLockscreenThemeSetting.Value != activeDesktopThemeSetting.Value)
                {
                    var activeLockscreenTheme = themeRepo.Find(activeLockscreenThemeSetting.Value.Value);
                    await fileDiscoveryService.PreformFileDiscovery(activeLockscreenTheme, null);

                }
                _deferral.Complete();
            }

            // Update the FileDiscovery Last Run
            _deferral = taskInstance.GetDeferral();
            var fileDiscoveryLastRunSetting = new FileDiscoveryLastRunSetting();
            fileDiscoveryLastRunSetting.Value = DateTime.UtcNow;
            _deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {

        }

        private void Task_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {

        }

        private void Task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {

        }
    }
}
