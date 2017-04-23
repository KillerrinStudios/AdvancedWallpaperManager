using KillerrinStudiosToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Storage;

namespace WallpaperManager.Models.Settings
{
    public class ActiveDesktopThemeHistorySetting : ApplicationSettingBase<Queue<string>>
    {
        public const int MAX_ITEMS = 5;
        /// <summary>
        /// In order to modify the list you must assign it to a temporary variable and re-assign to Value when complete
        /// </summary>
        public override Queue<string> Value
        {
            get { return JsonConvert.DeserializeObject<Queue<string>>((string)Container.Values[Key]); }
            set
            {
                // Limit the number of items on the Recent List
                while (value.Count > MAX_ITEMS) value.Dequeue();

                Container.Values[Key] = JsonConvert.SerializeObject(value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        public ActiveDesktopThemeHistorySetting()
            :base(StorageTask.LocalSettings, "ActiveDesktopThemeHistory", new Queue<string>())
        {
        }
    }
}
