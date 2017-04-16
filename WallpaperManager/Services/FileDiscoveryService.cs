using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Repositories;
using Windows.Storage;

namespace WallpaperManager.Services
{
    public class FileDiscoveryService : ServiceBase
    {

        StorageTask storageTask = new StorageTask();


        public FileDiscoveryService()
        {
        }

        public async Task<List<FileDiscoveryCache>> PreformFileDiscovery(IProgress<IndicatorProgressReport> progress)
        {
            List<FileDiscoveryCache> updatedThemeFilesCache = new List<FileDiscoveryCache>();
            using (WallpaperManagerContext context = new WallpaperManagerContext())
            {
                WallpaperThemeRepository themeRepo = new WallpaperThemeRepository(context);

                var allThemes = themeRepo.GetAll();
                foreach (var theme in allThemes)
                {
                    updatedThemeFilesCache.AddRange(await PreformFileDiscovery(theme, progress));
                }
            }

            return updatedThemeFilesCache;
        }
        public async Task<List<FileDiscoveryCache>> PreformFileDiscovery(WallpaperTheme theme, IProgress<IndicatorProgressReport> progress)
        {
            if (theme == null) return new List<FileDiscoveryCache>();
            if (theme.Directories.Count == 0) return new List<FileDiscoveryCache>();

            List<FileDiscoveryCache> updatedThemeFilesCache = new List<FileDiscoveryCache>();
            using (WallpaperManagerContext context = new WallpaperManagerContext())
            {
                WallpaperDirectoryRepository directoryRepo = new WallpaperDirectoryRepository(context);
                var directories = directoryRepo.GetAllQuery()
                    .Where(x => x.WallpaperThemeID == theme.ID)
                    .ToList();

                // Step one we have to populate our openedList with all the directories in the theme
                progress?.Report(new IndicatorProgressReport(true, 0.0, $"{nameof(FileDiscoveryService)} - {nameof(PreformFileDiscovery)} - {theme.Name} - Step 1/7", true));
                Stack<WallpaperDirectory> openedList = new Stack<WallpaperDirectory>(directories);

                // Grab the Excluded Paths
                var excludedList = directories
                    .Where(x => x.IsExcluded)
                    .Select(x => x.Path.ToLower())
                    .ToList();

                // Begin the discovery process
                while (openedList.Count > 0)
                {
                    var currentDirectory = openedList.Pop();
                    if (currentDirectory.IsExcluded) continue;

                    try
                    {
                        List<StorageFolder> allFolders = new List<StorageFolder>();

                        // Convert the path into a StorageFolder
                        var rootFolder = await StorageFolder.GetFolderFromPathAsync(currentDirectory.Path);
                        allFolders.Add(rootFolder);

                        // If we are allowed to gather subdirectories, gather them as well
                        if (currentDirectory.IncludeSubdirectories)
                        {
                            var subfoldersOpenedList = await StorageTask.Instance.GetDirectoryTreeFromFolder(rootFolder, false);
                            allFolders.AddRange(subfoldersOpenedList);
                        }

                        // Next go through all the folders and get their files
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
                updatedThemeFilesCache = updatedThemeFilesCache.OrderBy(x => x.FolderPath)
                    .ThenBy(x => x.FilePath)
                    .ToList();

                // Setup the File Cache Repo
                FileDiscoveryCacheRepository fileCacheRepo = new FileDiscoveryCacheRepository(context);

                // Before we upload to the repo, we need to clear the cache for the current theme
                var currentThemeCache = fileCacheRepo.GetAllQuery().Where(x => x.WallpaperThemeID == theme.ID).ToList();
                fileCacheRepo.RemoveRange(currentThemeCache);

                // With the current items out of the way, we can now add in our new items
                fileCacheRepo.AddRange(updatedThemeFilesCache);
            }

            return updatedThemeFilesCache;
        }
    }
}
