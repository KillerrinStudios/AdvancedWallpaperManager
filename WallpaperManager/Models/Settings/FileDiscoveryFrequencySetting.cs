using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Killerrin_Studios_Toolkit;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class FileDiscoveryFrequencySetting : ModelBase, IApplicationSetting<TimeSpan>
    {
        public ApplicationDataContainer Container { get; }

        public string Key { get { return "FileDiscoveryFrequency"; } }
        public TimeSpan Value
        {
            get { return (TimeSpan)Container.Values[Key]; }
            set {
                Container.Values[Key] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public FileDiscoveryFrequencySetting()
        {
            Container = StorageTask.LocalSettings;

            if (!Container.Values.ContainsKey(Key))
                Value = TimeSpan.FromHours(1.0);
        }

        public override string ToString()
        {
            return $"Application Setting: {Key} | {Value.ToString()}";
        }
    }
}
