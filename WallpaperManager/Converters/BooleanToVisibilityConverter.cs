using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WallpaperManager.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                string b = ((string)value);
                if (b == "true" || b == "True" || b == "TRUE")
                    return Visibility.Visible;
                else if (b == "false" || b == "False" || b == "FALSE")
                    return Visibility.Collapsed;
            }
            else if (value is bool)
            {
                bool b = ((bool)value);
                if (b)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility v = ((Visibility)value);

            if (v == Visibility.Visible) return true;
            else if (v == Visibility.Collapsed) return false;
            return true;
        }
    }
}
