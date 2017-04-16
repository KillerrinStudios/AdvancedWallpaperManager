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
    public class FileDiscoveryLastRunSetting : ModelBase, IApplicationSetting<DateTime>
    {
        public ApplicationDataContainer Container { get; }

        public string Key { get { return "FileDiscoveryLastRun"; } }
        public DateTime Value
        {
            get { return DateTime.ParseExact((string)Container.Values[Key], "MM/dd/yyyy hh:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture); }
            set
            {
                Container.Values[Key] = value.ToString("MM/dd/yyyy hh:mm:ss.fff");
                RaisePropertyChanged(nameof(Value));
            }
        }

        public FileDiscoveryLastRunSetting()
        {
            Container = StorageTask.LocalSettings;

            if (!Container.Values.ContainsKey(Key))
                Value = DateTime.MinValue;
        }

        public override string ToString()
        {
            return $"Application Setting: {Key} | {Value.ToString()}";
        }
    }
}
