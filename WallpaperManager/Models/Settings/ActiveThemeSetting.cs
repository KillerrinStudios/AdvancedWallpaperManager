using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Killerrin_Studios_Toolkit;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class ActiveThemeSetting : ModelBase, IApplicationSetting<int?>
    {
        public ApplicationDataContainer Container { get; }

        public string Key { get { return "ActiveTheme"; } }
        public int? Value
        {
            get { return (int?)Container.Values[Key]; }
            set
            {
                Container.Values[Key] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public ActiveThemeSetting()
        {
            Container = StorageTask.LocalSettings;

            if (!Container.Values.ContainsKey(Key))
                Value = null;
        }

        public override string ToString()
        {
            return $"Application Setting: {Key} | {Value.ToString()}";
        }
    }
}
