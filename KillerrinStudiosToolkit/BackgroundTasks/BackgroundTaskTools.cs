using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace KillerrinStudiosToolkit.BackgroundTasks
{
    public class BackgroundTaskTools
    {
        public bool IsTaskRegistered(string taskName)
        {
            return GetRegisteredTask(taskName) != null;
        }
        public IBackgroundTaskRegistration GetRegisteredTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return task.Value;
                }
            }

            return null;
        }

        #region GetAccessStatus
        public BackgroundAccessStatus GetAccessStatus() => BackgroundExecutionManager.GetAccessStatus();
        public BackgroundAccessStatus GetAccessStatus(string applicationID) => BackgroundExecutionManager.GetAccessStatus(applicationID); 
        #endregion

        #region RemoveAccess
        public void RemoveAccess() => BackgroundExecutionManager.RemoveAccess();
        public void RemoveAccess(string applicationID) => BackgroundExecutionManager.RemoveAccess(applicationID);
        #endregion

        #region IsAccessAllowed
        public bool IsAccessAllowed()
        {
            var backgroundTaskStatus = BackgroundExecutionManager.GetAccessStatus();
            return IsAccessAllowed(backgroundTaskStatus);
        }
        public bool IsAccessAllowed(string applicationID)
        {
            var backgroundTaskStatus = BackgroundExecutionManager.GetAccessStatus(applicationID);
            return IsAccessAllowed(backgroundTaskStatus);
        }
        public bool IsAccessAllowed(BackgroundAccessStatus accessStatus)
        {
            if (accessStatus == BackgroundAccessStatus.Unspecified ||
#pragma warning disable CS0618 // Type or member is obsolete
                accessStatus == BackgroundAccessStatus.Denied ||
#pragma warning restore CS0618 // Type or member is obsolete
                accessStatus == BackgroundAccessStatus.DeniedBySystemPolicy ||
                accessStatus == BackgroundAccessStatus.DeniedByUser)
                return false;
            return true;
        } 
        #endregion

        #region Request Access
        public async Task<bool> RequestAccess()
        {
            if (IsAccessAllowed())
                return true;

            var result = await BackgroundExecutionManager.RequestAccessAsync();
            return IsAccessAllowed();
        }
        public async Task<bool> RequestAccess(string applicationID)
        {
            if (IsAccessAllowed())
                return true;

            var result = await BackgroundExecutionManager.RequestAccessAsync(applicationID);
            return IsAccessAllowed();
        } 
        #endregion

    }
}
