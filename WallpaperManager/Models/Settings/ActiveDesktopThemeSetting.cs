using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Killerrin_Studios_Toolkit.Settings;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class ActiveDesktopThemeSetting : ApplicationSettingBase<int?>
    {
        public ActiveDesktopThemeSetting()
            :base(StorageTask.LocalSettings, "ActiveDesktopTheme", null)
        {
        }
    }
}
