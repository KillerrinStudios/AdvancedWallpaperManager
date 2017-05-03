using KillerrinStudiosToolkit.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models.Settings;
using Windows.ApplicationModel.Background;

namespace WallpaperManager.BackgroundTasks
{
    public class ActiveThemeBackgroundTaskHelper : BackgroundTaskHelperBase
    {
        public ActiveThemeBackgroundTaskHelper()
            :base("ActiveTheme", $"{nameof(WallpaperManager)}.{nameof(BackgroundTasks)}.{nameof(ActiveThemeBackgroundTask)}")
        {

        }

        protected override BackgroundTaskBuilder BuildTask()
        {
            var builder = new BackgroundTaskBuilder();
            builder.Name = TaskName;
            builder.TaskEntryPoint = TaskEntryPoint;

            builder.SetTrigger(new TimeTrigger(15, false));

            return builder;
        }

        protected override bool CancelTaskRegistration()
        {
            return false;
        }
    }
}
