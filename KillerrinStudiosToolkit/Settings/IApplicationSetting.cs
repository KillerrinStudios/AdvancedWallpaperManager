using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace KillerrinStudiosToolkit.Settings
{
    public interface IApplicationSetting<T>
    {
        ApplicationDataContainer Container { get; }

        string Key { get; }
        T DefaultValue { get; set; }
        T Value { get; set; }
    }

}
