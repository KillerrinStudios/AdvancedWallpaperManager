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
    public class ActiveThemeBackgroundTaskHelper : BackgroundTaskHelperBase
    {
        ActiveThemeTaskLastRunSetting m_activeThemeLastRun = new ActiveThemeTaskLastRunSetting();

        public ActiveThemeBackgroundTaskHelper()
            :base("ActiveTheme", $"{nameof(AdvancedWallpaperManager)}.{nameof(BackgroundTasks)}.{nameof(ActiveThemeBackgroundTask)}")
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
