using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Storage;

namespace AdvancedWallpaperManager.Models.Settings
{
    public class ActiveLockscreenThemeCurrentWallpaperSetting : ApplicationSettingBase<string>
    {
        public ActiveLockscreenThemeCurrentWallpaperSetting()
            :base(StorageTask.LocalSettings, "ActiveLockscreenThemeCurrentWallpaper", "")
        {
        }
    }
}
