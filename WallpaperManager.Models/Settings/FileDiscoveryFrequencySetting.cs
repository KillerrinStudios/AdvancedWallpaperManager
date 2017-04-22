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
    public class FileDiscoveryFrequencySetting : ApplicationSettingBase<TimeSpan>
    {
        public FileDiscoveryFrequencySetting()
            :base(StorageTask.LocalSettings, "FileDiscoveryFrequency", TimeSpan.FromHours(1.0))
        {
        }
    }
}
