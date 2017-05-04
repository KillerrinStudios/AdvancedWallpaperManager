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
    public class ActiveDesktopThemeSetting : ApplicationSettingBase<int?>
    {
        public ActiveDesktopThemeSetting()
            :base(StorageTask.LocalSettings, "ActiveDesktopTheme", null)
        {
        }
    }
}
