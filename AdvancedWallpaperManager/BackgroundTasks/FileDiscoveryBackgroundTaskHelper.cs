using KillerrinStudiosToolkit.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWallpaperManager.Models.Settings;
using Windows.ApplicationModel.Background;

namespace AdvancedWallpaperManager.BackgroundTasks
{
    public class FileDiscoveryBackgroundTaskHelper : BackgroundTaskHelperBase
    {
        public FileDiscoveryBackgroundTaskHelper()
            :base("FileDiscovery", $"{nameof(AdvancedWallpaperManager)}.{nameof(BackgroundTasks)}.{nameof(FileDiscoveryBackgroundTask)}")
        {

        }

        protected override BackgroundTaskBuilder BuildTask()
        {
            var builder = new BackgroundTaskBuilder();
            builder.Name = TaskName;
            builder.TaskEntryPoint = TaskEntryPoint;

            var fileDiscoveryFrequency = new FileDiscoveryFrequencySetting();
            builder.SetTrigger(new MaintenanceTrigger((uint)fileDiscoveryFrequency.Value.TotalMinutes, false));
            //builder.SetTrigger(new TimeTrigger((uint)fileDiscoveryFrequency.Value.TotalMinutes, false));

            return builder;
        }

        protected override bool CancelTaskRegistration()
        {
            return false;
        }
    }
}
