﻿using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class ActiveDesktopThemeCurrentWallpaperSetting : ApplicationSettingBase<string>
    {
        public ActiveDesktopThemeCurrentWallpaperSetting()
            :base(StorageTask.LocalSettings, "ActiveDesktopThemeCurrentWallpaper", "")
        {
        }
    }
}
