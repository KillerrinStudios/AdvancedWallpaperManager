using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using Windows.Foundation.Collections;
using Windows.Storage;
using System.Diagnostics;

namespace AdvancedWallpaperManager.Models.Settings
{
    public class ActiveDesktopThemeNextRunSetting : ApplicationSettingBase<DateTime>
    {
        public override DateTime Value
        {
            get
            {
                if (DateTime.TryParse((string)Container.Values[Key],
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal,
                    out DateTime result))
                {
                    return result;
                }

                RevertToDefault();
                return DefaultValue;
            }
            set
            {
                Container.Values[Key] = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                RaiseValuePropertyChanged();
            }
        }
        public DateTime ValueLocalTime
        {
            get { return Value.ToLocalTime(); }
            set { Value = value.ToUniversalTime(); }
        }

        public string ValueAsString
        {
            get { return Value.ToString(); }
        }
        public string ValueLocalTimeAsString
        {
            get { return ValueLocalTime.ToString(); }
        }

        public ActiveDesktopThemeNextRunSetting()
            : base(StorageTask.LocalSettings, "ActiveDesktopThemeNextRun", DateTime.MinValue)
        {
        }

        public override void RaiseValuePropertyChanged()
        {
            RaisePropertyChanged(nameof(Value));
            RaisePropertyChanged(nameof(ValueLocalTime));
            RaisePropertyChanged(nameof(ValueAsString));
            RaisePropertyChanged(nameof(ValueLocalTimeAsString));
        }
    }
}
