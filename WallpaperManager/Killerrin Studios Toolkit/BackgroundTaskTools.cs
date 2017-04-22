using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace WallpaperManager.Killerrin_Studios_Toolkit
{
    public class BackgroundTaskTools
    {
        public bool IsTaskRegistered(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }

            return false;
        }

        public BackgroundAccessStatus GetAccessStatus() => BackgroundExecutionManager.GetAccessStatus();
        public void RemoveAccess() => BackgroundExecutionManager.RemoveAccess();

        public bool IsAccessAllowed()
        {
            var backgroundTaskStatus = BackgroundExecutionManager.GetAccessStatus();
            if (backgroundTaskStatus == BackgroundAccessStatus.Denied ||
                backgroundTaskStatus == BackgroundAccessStatus.DeniedBySystemPolicy ||
                backgroundTaskStatus == BackgroundAccessStatus.DeniedByUser ||
                backgroundTaskStatus == BackgroundAccessStatus.Unspecified)
                return false;
            return true;
        }

        public async Task<bool> RequestAccess()
        {
            if (IsAccessAllowed())
                return true;

            var result = await BackgroundExecutionManager.RequestAccessAsync();
            return IsAccessAllowed();
        }

    }
}
