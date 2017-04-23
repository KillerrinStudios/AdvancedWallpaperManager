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

        public override TimeSpan Value
        {
            get { return (TimeSpan)Container.Values[Key]; }
            set
            {
                // Due to Background Tasks limitations, the value must be >= 15 Minutes in order to successfully work
                if (value < TimeSpan.FromMinutes(15)) return;

                Container.Values[Key] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public FileDiscoveryFrequencySetting()
            :base(StorageTask.LocalSettings, "FileDiscoveryFrequency", TimeSpan.FromHours(1.0))
        {
        }
    }
}
