using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace WallpaperManager.Killerrin_Studios_Toolkit
{
    public interface IApplicationSetting<T>
    {
        ApplicationDataContainer Container { get; }

        string Key { get; }
        T Value { get; set; }

    }
}
