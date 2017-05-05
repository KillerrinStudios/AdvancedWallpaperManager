using KillerrinStudiosToolkit.Services;
using KillerrinStudiosToolkit.UserProfile;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWallpaperManager.Models;
using AdvancedWallpaperManager.Models.Settings;
using AdvancedWallpaperManager.DAL.Repositories;
using KillerrinStudiosToolkit;

namespace AdvancedWallpaperManager.Services
{
    public class ActiveThemeService : ServiceBase
    {
        WallpaperManager m_wallpaperManager = new WallpaperManager();
        ActiveDesktopThemeSetting m_activeDesktopThemeSetting = new ActiveDesktopThemeSetting();
        ActiveDesktopThemeCurrentWallpaperSetting m_activeDesktopThemeCurrentWallpaperSetting = new ActiveDesktopThemeCurrentWallpaperSetting();
        ActiveDesktopThemeHistorySetting m_activeDesktopThemeHistorySetting = new ActiveDesktopThemeHistorySetting();
        ActiveDesktopThemeNextRunSetting m_activeDesktopThemeNextRunSetting = new ActiveDesktopThemeNextRunSetting();

        LockscreenManager m_lockscreenManager = new LockscreenManager();
        ActiveLockscreenThemeSetting m_activeLockscreenThemeSetting = new ActiveLockscreenThemeSetting();
        ActiveLockscreenThemeCurrentWallpaperSetting m_activeLockscreenThemeCurrentWallpaperSetting = new ActiveLockscreenThemeCurrentWallpaperSetting();
        ActiveLockscreenThemeHistorySetting m_activeLockscreenThemeHistorySetting = new ActiveLockscreenThemeHistorySetting();
        ActiveLockscreenThemeNextRunSetting m_activeLockscreenThemeNextRunSetting = new ActiveLockscreenThemeNextRunSetting();

        public ActiveThemeService()
        {

        }

        #region Change
        public void ChangeActiveDesktopTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            DeselectActiveDesktopTheme();
            m_activeDesktopThemeSetting.Value = theme.ID;
        }
        public void ChangeActiveLockscreenTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            DeselectActiveLockscreenTheme();
            m_activeLockscreenThemeSetting.Value = theme.ID;
        }
        #endregion

        #region Deselect
        public void DeselectActiveDesktopTheme()
        {
            if (!ServiceEnabled) return;
            m_activeDesktopThemeSetting.RevertToDefault();
            m_activeDesktopThemeCurrentWallpaperSetting.RevertToDefault();
            m_activeDesktopThemeHistorySetting.RevertToDefault();
            m_activeDesktopThemeNextRunSetting.RevertToDefault();

            m_wallpaperManager.DeleteFilesInFolders();
        }
        public void DeselectActiveLockscreenTheme()
        {
            if (!ServiceEnabled) return;
            m_activeLockscreenThemeSetting.RevertToDefault();
            m_activeLockscreenThemeCurrentWallpaperSetting.RevertToDefault();
            m_activeLockscreenThemeHistorySetting.RevertToDefault();
            m_activeLockscreenThemeNextRunSetting.RevertToDefault();
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

            var current = m_activeDesktopThemeCurrentWallpaperSetting.Value;
            
            // If the Cache was empty or cleared, grab the first image
            if (string.IsNullOrWhiteSpace(current))
                return activeDesktopTheme.FirstImageFromCache;

            return current;
        }
        public string GetCurrentLockscreenImagePath()
        {
            var activeLockscreenTheme = GetActiveLockscreenTheme();
            if (activeLockscreenTheme == null) return "";

            var current = m_activeLockscreenThemeCurrentWallpaperSetting.Value;

            // If the Cache was empty or cleared, grab the first image
            if (string.IsNullOrWhiteSpace(current))
                return activeLockscreenTheme.FirstImageFromCache;

            return current;
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

            // If the index cant be found, give us a random image
            if (currentImageIndex == -1)
                return activeDesktopTheme.RandomImageFromCache;

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
            // Get our Current Lockscreen Image
            var currentImagePath = GetCurrentLockscreenImagePath();

            // Find the Index of it in the cache
            FileDiscoveryService service = new FileDiscoveryService();
            var cache = service.GetCache(activeLockscreenTheme);
            var currentImageIndex = cache.FindIndex(x => x.FilePath == currentImagePath);

            // If the index cant be found, give us a random image
            if (currentImageIndex == -1)
                return activeLockscreenTheme.RandomImageFromCache;

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
            var activeDesktopTheme = GetActiveDesktopTheme();
            if (activeDesktopTheme == null) return;

            // Get the Storage File
            StorageTask storageTask = new StorageTask();
            var file = await storageTask.GetFileFromPath(new Uri(path));
            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Converted Path to File: {file.Name}");

            // Set the Wallpaper
            if (await m_wallpaperManager.SetImage(file))
            {
                // Add it to the history if successful
                m_activeDesktopThemeHistorySetting.Add(path);
                m_activeDesktopThemeCurrentWallpaperSetting.Value = path;
                m_activeDesktopThemeNextRunSetting.Value = DateTime.UtcNow.Add(activeDesktopTheme.WallpaperChangeFrequency);
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Successfully Changed Image");
            }
            else
            {
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextDesktopBackground)} - Failed");

                if (DebugTools.DebugMode)
                    m_activeDesktopThemeHistorySetting.Add(path); // REMOVEME: DEBUGGING PURPOSES
            }
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
            var activeLockscreenTheme = GetActiveLockscreenTheme();
            if (activeLockscreenTheme == null) return;

            Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Setting Path: {path}");

            // Set the Lockscreen
            if (await m_lockscreenManager.SetImageFromFileSystem(path))
            {
                // Add it to the history if successful
                m_activeLockscreenThemeHistorySetting.Add(path);
                m_activeLockscreenThemeCurrentWallpaperSetting.Value = path;
                m_activeLockscreenThemeNextRunSetting.Value = DateTime.UtcNow.Add(activeLockscreenTheme.WallpaperChangeFrequency);
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Successfully Changed Image");
            }
            else
            {
                Debug.WriteLine($"{nameof(ActiveThemeService)}.{nameof(NextLockscreenBackground)} - Failed");

                if (DebugTools.DebugMode)
                    m_activeLockscreenThemeHistorySetting.Add(path); // REMOVEME: DEBUGGING PURPOSES
            }
        }
        #endregion

    }
}
