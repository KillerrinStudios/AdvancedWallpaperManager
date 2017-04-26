using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit;
using WallpaperManager.Models;
using WallpaperManager.DAL;
using Windows.Storage;
using WallpaperManager.DAL.Repositories;
using WallpaperManager.Models.Enums;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace WallpaperManager.Services
{
    public class WindowsThemeService
    {
        public const string Theme_File_Extension = ".theme";
        StorageTask m_storageTask = new StorageTask();

        public WindowsThemeService()
        {

        }

        public async Task<StorageFile> CreateWindowsTheme(WallpaperTheme theme)
        {
            using (var context = new WallpaperManagerContext())
            {
                var fileDiscoveryService = new FileDiscoveryService(context);

                var fileCache = fileDiscoveryService.GetCache(theme);
                if (fileCache.Count <= 0) return null;

                // Build up the file format
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("; Copyright © Killerrin Studios");
                builder.AppendLine();
                builder.AppendLine("[Theme]");
                builder.AppendLine($"DisplayName={theme.Name}");
                builder.AppendLine();
                builder.AppendLine(@"[Control Panel\Desktop]");
                builder.AppendLine($"Wallpaper={theme.FirstImageFromCache}");
                builder.AppendLine("; The path to the wallpaper picture can point to a .bmp, .gif, .jpg, .png, or .tif file.");
                builder.AppendLine("Pattern=");
                //builder.AppendLine($"MultimonBackgrounds={0}");
                //builder.AppendLine($"PicturePosition={4}");
                builder.AppendLine($"TileWallpaper={0}");
                builder.AppendLine("; 0: The wallpaper picture should not be tiled");
                builder.AppendLine("; 1: The wallpaper picture should be tiled ");
                builder.AppendLine($"WallpaperStyle={10}");
                builder.AppendLine("; 0:  The image is centered if TileWallpaper = 0 or tiled if TileWallpaper = 1");
                builder.AppendLine("; 2:  The image is stretched to fill the screen");
                builder.AppendLine("; 6:  The image is resized to fit the screen while maintaining the aspect ratio. (Windows 7 and later)");
                builder.AppendLine("; 10: The image is resized and cropped to fill the screen while maintaining the aspect ratio. (Windows 7 and later)");
                builder.AppendLine();
                builder.AppendLine("[Slideshow]");
                builder.AppendLine($"Interval={theme.WallpaperChangeFrequency.TotalMilliseconds}");
                builder.AppendLine("; Interval is a number that determines how often the background changes. It is measured in milliseconds.");
                switch (theme.WallpaperSelectionMethod)
                {
                    case ImageSelectionMethod.Random:
                    case ImageSelectionMethod.Sequential:
                        builder.AppendLine($"Shuffle={(int)theme.WallpaperSelectionMethod}");
                        break;
                    default:
                        builder.AppendLine($"Shuffle={0}");
                        break;
                }
                builder.AppendLine("; 0: Disabled");
                builder.AppendLine("; 1: Enabled");
                builder.AppendLine();
                builder.AppendLine($"ImagesRootPath={fileCache[0].FolderPath}");
                for (int i = 0; i < fileCache.Count; i++)
                {
                    builder.AppendLine($"Item{i}Path={fileCache[i].FilePath}");
                }
                builder.AppendLine();
                builder.AppendLine("[VisualStyles]");
                builder.AppendLine($@"Path=%ResourceDir%\Themes\Aero\Aero.msstyles");
                builder.AppendLine($"ColorStyle=NormalColor");
                builder.AppendLine($"Size=NormalSize");
                builder.AppendLine($"AutoColorization=0");
                builder.AppendLine($"ColorizationColor=0XC40078D7");
                builder.AppendLine();
                builder.AppendLine("[boot]");
                builder.AppendLine("SCRNSAVE.EXE=");
                builder.AppendLine();
                builder.AppendLine("[MasterThemeSelector]");
                builder.AppendLine("MTSM=RJSPBS");

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("Windows Theme File", new List<string>() { ".theme" });
                savePicker.SuggestedFileName = $"{theme.Name}";

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);
                    await FileIO.WriteTextAsync(file, builder.ToString());
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == FileUpdateStatus.Complete || status == FileUpdateStatus.CompleteAndRenamed)
                        return file;
                }
            }

            return null;
        }
    }
}
