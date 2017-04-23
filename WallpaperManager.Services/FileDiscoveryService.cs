using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Models;
using KillerrinStudiosToolkit.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using WallpaperManager.DAL.Repositories;
using Windows.Storage;

namespace WallpaperManager.Services
{
    public class FileDiscoveryService : ServiceBase, IDisposable
    {
        public bool ShouldDispose { get { return createdContext; } }

        StorageTask storageTask = new StorageTask();
        IProgress<IndicatorProgressReport> allCacheProgress = null;

        // Cache the Repositories for later
        bool createdContext = false;
        WallpaperManagerContext Context;
        WallpaperThemeRepository ThemeRepo;
        WallpaperDirectoryRepository DirectoryRepo;
        FileDiscoveryCacheRepository FileCacheRepo;

        public FileDiscoveryService()
            : this(new WallpaperManagerContext())
        {
            createdContext = true;
        }

        public FileDiscoveryService(WallpaperManagerContext context)
        {
            Context = context;
            ThemeRepo = new WallpaperThemeRepository(context);
            DirectoryRepo = new WallpaperDirectoryRepository(context);
            FileCacheRepo = new FileDiscoveryCacheRepository(context);
        }

        public void Dispose()
        {
            if (createdContext)
                Context.Dispose();
        }

        public async Task<List<FileDiscoveryCache>> PreformFileDiscoveryAll(IProgress<IndicatorProgressReport> progress)
        {
            // Cache the old Progress Variable
            allCacheProgress = progress;

            // Create a Progress Wrapper so we don't end up turning off the progress indicator by accident
            Progress<IndicatorProgressReport> internalProgress = new Progress<IndicatorProgressReport>();
            internalProgress.ProgressChanged += new EventHandler<IndicatorProgressReport>(delegate (object sender, IndicatorProgressReport e)
            {
                double percentage = e.Percentage;
                if (percentage >= 100.0) percentage -= 1.0;
                allCacheProgress?.Report(new IndicatorProgressReport(e.RingEnabled, percentage, e.StatusMessage, e.WriteToDebugConsole));
            });

            // Clear the FileDiscoveryCache
            progress?.Report(new IndicatorProgressReport(true, 0.0, $"Clearing the Cache - 1/1", true));
            FileCacheRepo.Clear();

            // Go through all the themes and begin the caching process
            List<FileDiscoveryCache> updatedThemeFilesCache = new List<FileDiscoveryCache>();
            var allThemes = ThemeRepo.GetAll();
            
            // Update the Cache Discovery Date
            foreach (var theme in allThemes)
            {
                theme.DateCacheDiscovered = DateTime.UtcNow;
                ThemeRepo.UpdateAndCommit(theme);
            }

            // Update the Cache
            foreach (var theme in allThemes)
            {
                var cache = await PreformFileDiscovery(theme, internalProgress);
                updatedThemeFilesCache.AddRange(cache);
            }

            // Update the Last Run
            var fileDiscoveryLastRunSetting = new FileDiscoveryLastRunSetting();
            fileDiscoveryLastRunSetting.Value = DateTime.UtcNow;

            // Report back completion and return
            allCacheProgress = null;
            progress?.Report(new IndicatorProgressReport(true, 100.0, $"File Discovery Completed", true));
            return updatedThemeFilesCache;
        }

        public async Task<List<FileDiscoveryCache>> PreformFileDiscovery(WallpaperTheme theme, IProgress<IndicatorProgressReport> progress)
        {
            if (theme == null) return new List<FileDiscoveryCache>();

            List<FileDiscoveryCache> updatedThemeFilesCache = new List<FileDiscoveryCache>();

            progress?.Report(new IndicatorProgressReport(true, 0.0, $"Grabbing Theme directories - {theme.Name} - Step 1/1", true));
            var directories = DirectoryRepo.GetAllQuery()
                .Where(x => x.WallpaperThemeID == theme.ID)
                .ToList();

            if (directories.Count == 0) return new List<FileDiscoveryCache>();

            // Step one we have to populate our openedList with all the directories in the theme
            Stack<WallpaperDirectory> openedList = new Stack<WallpaperDirectory>(directories);

            // Grab the Excluded Paths
            var excludedList = directories
                .Where(x => x.IsExcluded)
                .Select(x => x.Path.ToLower())
                .ToList();

            // Mark the File Discovery Date on the Theme and Update it
            if (allCacheProgress == null)
            {
                theme.DateCacheDiscovered = DateTime.UtcNow;
                ThemeRepo.UpdateAndCommit(theme);
            }

            // Begin the discovery process
            while (openedList.Count > 0)
            {
                var currentDirectory = openedList.Pop();
                if (currentDirectory.IsExcluded) continue;

                try
                {
                    List<StorageFolder> allFolders = new List<StorageFolder>();

                    // Convert the path into a StorageFolder
                    progress?.Report(new IndicatorProgressReport(true, 20.0, $"Discovering Folders For Theme - {theme.Name} - Step 1/3", true));
                    var rootFolder = await StorageFolder.GetFolderFromPathAsync(currentDirectory.Path);
                    allFolders.Add(rootFolder);

                    // If we are allowed to gather subdirectories, gather them as well
                    if (currentDirectory.IncludeSubdirectories)
                    {
                        progress?.Report(new IndicatorProgressReport(true, 30.0, $"Discovering Subfolders For Theme - {theme.Name} - Step 2/3", true));
                        var subfoldersOpenedList = await StorageTask.Instance.GetDirectoryTreeFromFolder(rootFolder, false);
                        allFolders.AddRange(subfoldersOpenedList);
                    }

                    // Next go through all the folders and get their files
                    progress?.Report(new IndicatorProgressReport(true, 40.0, $"Discovering Files For Theme - {theme.Name} - Step 3/3", true));
                    foreach (var currentFolder in allFolders)
                    {
                        try
                        {
                            var files = await currentFolder.GetFilesAsync();
                            foreach (var currentFile in files)
                            {
                                if (currentFile.ContentType.ToLower().Contains("image"))
                                {
                                    FileDiscoveryCache cache = new FileDiscoveryCache();
                                    cache.WallpaperThemeID = currentDirectory.WallpaperThemeID;
                                    cache.FileAccessTokenID = currentDirectory.FileAccessTokenID;
                                    cache.StorageLocation = currentDirectory.StorageLocation;
                                    cache.FolderPath = currentFolder.Path;
                                    cache.FilePath = currentFile.Path;
                                    cache.DateDiscovered = theme.DateCacheDiscovered;
                                    updatedThemeFilesCache.Add(cache);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                            if (Debugger.IsAttached)
                                Debugger.Break();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    if (Debugger.IsAttached)
                        Debugger.Break();
                }
            }

            // Check the exclusion list and remove what shouldn't exist
            progress?.Report(new IndicatorProgressReport(true, 60.0, $"Excluding Items - {theme.Name} - Step 1/1", true));
            for (int i = updatedThemeFilesCache.Count - 1; i >= 0; i--)
            {
                foreach (var excludedPath in excludedList)
                {
                    if (updatedThemeFilesCache[i].FilePath.ToLower().Contains(excludedPath))
                    {
                        updatedThemeFilesCache.RemoveAt(i);
                        break;
                    }
                }
            }

            // Order the Cache
            progress?.Report(new IndicatorProgressReport(true, 70.0, $"Ordering Cache - {theme.Name} - Step 1/1", true));
            updatedThemeFilesCache = updatedThemeFilesCache.OrderBy(x => x.FolderPath)
                .ThenBy(x => x.FilePath)
                .ToList();

            // Before we clear the cache, make sure we aren't running within the scope of PreformFileDiscoveryAll
            if (allCacheProgress == null)
            {
                // Before we upload to the repo, we need to clear the cache for the current theme
                progress?.Report(new IndicatorProgressReport(true, 80.0, $"Clearing Old Cache - {theme.Name} - Step 1/2", true));
                var currentThemeCache = FileCacheRepo.GetAllQuery().Where(x => x.WallpaperThemeID == theme.ID).ToList();
                FileCacheRepo.RemoveRange(currentThemeCache);
            }

            // With the current items out of the way, we can now add in our new items
            progress?.Report(new IndicatorProgressReport(true, 90.0, $"Updating Cache - {theme.Name} - Step 2/2", true));
            FileCacheRepo.AddRange(updatedThemeFilesCache);

            progress?.Report(new IndicatorProgressReport(true, 100.0, $"Completed Cache For - {theme.Name}", true));
            return updatedThemeFilesCache;
        }
    }
}
