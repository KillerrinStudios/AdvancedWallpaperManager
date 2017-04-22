using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class FileDiscoveryEnableSetting : ApplicationSettingBase<bool>
    {
        public FileDiscoveryEnableSetting()
            :base(StorageTask.LocalSettings, "FileDiscoveryEnable", false)
        {
        }
    }
}
