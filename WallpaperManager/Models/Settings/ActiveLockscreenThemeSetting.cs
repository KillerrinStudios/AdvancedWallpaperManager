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
    public class ActiveLockscreenThemeSetting : ApplicationSettingBase<int?>
    {
        public ActiveLockscreenThemeSetting()
            :base(StorageTask.LocalSettings, "ActiveLockscreenTheme", null)
        {
        }
    }
}
