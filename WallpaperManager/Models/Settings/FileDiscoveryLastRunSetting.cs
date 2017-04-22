using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Killerrin_Studios_Toolkit.Settings;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class FileDiscoveryLastRunSetting : ApplicationSettingBase<DateTime>
    {
        public override DateTime Value
        {
            get { return DateTime.ParseExact((string)Container.Values[Key], "MM/dd/yyyy hh:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture); }
            set
            {
                Container.Values[Key] = value.ToString("MM/dd/yyyy hh:mm:ss.fff");
                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(ValueAsString));
            }
        }
        public string ValueAsString
        {
            get { return (string)Container.Values[Key]; }
        }

        public FileDiscoveryLastRunSetting()
            :base(StorageTask.LocalSettings, "FileDiscoveryLastRun", DateTime.MinValue)
        {
        }
    }
}
