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
    public class FileDiscoveryEnableSetting : ModelBase, IApplicationSetting<bool>
    {
        public ApplicationDataContainer Container { get; }

        public string Key { get { return "FileDiscoveryEnable"; } }
        public bool Value
        {
            get { return (bool)Container.Values[Key]; }
            set {
                Container.Values[Key] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public FileDiscoveryEnableSetting()
        {
            Container = StorageTask.LocalSettings;

            if (!Container.Values.ContainsKey(Key))
                Value = false;
        }

        public override string ToString()
        {
            return $"Application Setting: {Key} | {Value.ToString()}";
        }
    }
}
