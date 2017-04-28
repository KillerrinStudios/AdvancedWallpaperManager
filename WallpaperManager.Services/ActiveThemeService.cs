using KillerrinStudiosToolkit.Services;
using KillerrinStudiosToolkit.UserProfile;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using WallpaperManager.DAL.Repositories;
using KillerrinStudiosToolkit;

namespace WallpaperManager.Services
{
    public class ActiveThemeService : ServiceBase
    {
        KillerrinStudiosToolkit.UserProfile.WallpaperManager m_wallpaperManager = new KillerrinStudiosToolkit.UserProfile.WallpaperManager();
        ActiveDesktopThemeSetting m_activeDesktopThemeSetting = new ActiveDesktopThemeSetting();
        ActiveDesktopThemeHistorySetting m_activeDesktopThemeHistorySetting = new ActiveDesktopThemeHistorySetting();

        LockscreenManager m_lockscreenManager = new LockscreenManager();
        ActiveLockscreenThemeSetting m_activeLockscreenThemeSetting = new ActiveLockscreenThemeSetting();
        ActiveLockscreenThemeHistorySetting m_activeLockscreenThemeHistorySetting = new ActiveLockscreenThemeHistorySetting();

        public ActiveThemeService()
        {

        }

        #region Change
        public void ChangeActiveDesktopTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            m_activeDesktopThemeSetting.Value = theme.ID;
            m_activeDesktopThemeHistorySetting.RevertToDefault();
            m_wallpaperManager.DeleteFilesInFolders();
        }
        public void ChangeActiveLockscreenTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            m_activeLockscreenThemeSetting.Value = theme.ID;
            m_activeLockscreenThemeHistorySetting.RevertToDefault();
            m_lockscreenManager.DeleteFilesInFolders();
        }
        #endregion

        #region Deselect
        public void DeselectActiveDesktopTheme()
        {
            if (!ServiceEnabled) return;
            m_activeDesktopThemeSetting.RevertToDefault();
            m_activeDesktopThemeHistorySetting.RevertToDefault();
            m_wallpaperManager.DeleteFilesInFolders();
        }
        public void DeselectActiveLockscreenTheme()
        {
            if (!ServiceEnabled) return;
            m_activeLockscreenThemeSetting.RevertToDefault();
            m_activeLockscreenThemeHistorySetting.RevertToDefault();
            m_lockscreenManager.DeleteFilesInFolders();
        }
        #endregion

        #region Get Theme
        public int? GetActiveDesktopThemeID() { return m_activeDesktopThemeSetting.Value; }
        public WallpaperTheme GetActiveDesktopTheme()
        {
            var value = m_activeDesktopThemeSetting.Value;
            if (!value.HasValue) return null;

            return GetTheme(value.Value);
        }

        public int? GetActiveLockscreenThemeID() { return m_activeLockscreenThemeSetting.Value; }
        public WallpaperTheme GetActiveLockscreenTheme()
        {
            var value = m_activeLockscreenThemeSetting.Value;
            if (!value.HasValue) return null;

            return GetTheme(value.Value);
        }
        private WallpaperTheme GetTheme(int id)
        {
            using (var context = new WallpaperManagerContext())
            {
                var themeRepo = new WallpaperThemeRepository(context);

                try { return themeRepo.Find(id); }
                catch (Exception) { }
            }

            return null;
        }
        #endregion

        #region Current Image Path
        public string GetCurrentDesktopImagePath()
        {
            var activeDesktopTheme = GetActiveDesktopTheme();
            if (activeDesktopTheme == null) return "";

            // If the Recent hasn't been messed with, grab the current image from the history
            var history = m_activeDesktopThemeHistorySetting.Value;
            if (history.Count > 0)
                return history[0];

            // If the Cache was empty or cleared, grab the first image
            return activeDesktopTheme.FirstImageFromCache;
        }
        public string GetCurrentLockscreenImagePath()
        {
            var activeLockscreenTheme = GetActiveLockscreenTheme();
            if (activeLockscreenTheme == null) return "";

            // If the Recent hasn't been messed with, grab the current image from the history
            var history = m_activeLockscreenThemeHistorySetting.Value;
            if (history.Count > 0)
                return history[0];

            // If the Cache was empty or cleared, grab the first image
            return activeLockscreenTheme.FirstImageFromCache;
        }
        #endregion

        #region Next Image Path
        public string GetNextDesktopImagePath()
        {
            var activeDesktopTheme = GetActiveDesktopTheme();
            if (activeDesktopTheme == null) return "";

            // If the SelectionMethod is Random, return a random image
            if (activeDesktopTheme.WallpaperSelectionMethod == Models.Enums.ImageSelectionMethod.Random)
                return activeDesktopTheme.RandomImageFromCache;

            // Otherwise, grab the next item on the stack
            // Get our Current Desktop Image
            var currentImagePath = GetCurrentDesktopImagePath();

            // Find the Index of it in the cache
            FileDiscoveryService service = new FileDiscoveryService();
            var cache = service.GetCache(activeDesktopTheme);
            var currentImageIndex = cache.FindIndex(x => x.FilePath == currentImagePath);

            // If we go over the cache count, return the first item in the Cache, otherwise return the next element
            if ((currentImageIndex + 1) >= cache.Count)
                return cache[0].FilePath;
            return cache[currentImageIndex + 1].FilePath;
        }

        public string GetNextLockscreenImagePath()
        {
            var activeLockscreenTheme = GetActiveLockscreenTheme();
            if (activeLockscreenTheme == null) return "";

            // If the SelectionMethod is Random, return a random image
            if (activeLockscreenTheme.WallpaperSelectionMethod == Models.Enums.ImageSelectionMethod.Random)
                return activeLockscreenTheme.RandomImageFromCache;

            // Otherwise, grab the next item on the stack
            // Get our Current Desktop Image
            var currentImagePath = GetCurrentDesktopImagePath();

            // Find the Index of it in the cache
            FileDiscoveryService service = new FileDiscoveryService();
            var cache = service.GetCache(activeLockscreenTheme);
            var currentImageIndex = cache.FindIndex(x => x.FilePath == currentImagePath);

            // If we go over the cache count, return the first item in the Cache, otherwise return the next element
            if ((currentImageIndex + 1) >= cache.Count)
                return cache[0].FilePath;
            return cache[currentImageIndex + 1].FilePath;
        }
        #endregion

        #region Change Image
        public async Task NextDesktopBackground()
        {
            var activeDesktopTheme = GetActiveDesktopTheme();
            if (activeDesktopTheme == null) return;
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Begin");

            // Get the Next Image
            var nextImagePath = GetNextDesktopImagePath();
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Next Image Selected: {nextImagePath}");

            await NextDesktopBackground(nextImagePath);
        }
        public async Task NextDesktopBackground(string path)
        {
            // Get the Storage File
            StorageTask storageTask = new StorageTask();
            var file = await storageTask.GetFileFromPath(new Uri(path));
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Converted Path to File: {file.Name}");

            // Set the Wallpaper
            m_activeDesktopThemeHistorySetting.Add(path); // REMOVEME: DEBUGGING PURPOSES
            if (await m_wallpaperManager.SetImage(file))
            {
                // Add it to the history if successful
                m_activeDesktopThemeHistorySetting.Add(path);
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Successfully Changed Image");
            }
            else { Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Failed"); }
        }

        public async Task NextLockscreenBackground()
        {
            var activeLockscreenTheme = GetActiveLockscreenTheme();
            if (activeLockscreenTheme == null) return;
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Begin");

            // Get the Next Image
            var nextImagePath = GetNextLockscreenImagePath();
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Next Image Selected: {nextImagePath}");

            await NextLockscreenBackground(nextImagePath);
        }
        public async Task NextLockscreenBackground(string path)
        {
            // Set the Lockscreen
            m_activeLockscreenThemeHistorySetting.Add(path); // REMOVEME: DEBUGGING PURPOSES
            if (await m_lockscreenManager.SetImageFromFileSystem(path))
            {
                // Add it to the history if successful
                m_activeLockscreenThemeHistorySetting.Add(path);
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Successfully Changed Image");
            }
            else { Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Failed"); }
        }
        #endregion
    }
}
