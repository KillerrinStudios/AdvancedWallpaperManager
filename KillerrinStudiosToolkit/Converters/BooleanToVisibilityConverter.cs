using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace KillerrinStudiosToolkit.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                string b = ((string)value).ToLower();
                if (b == "true")
                    return Visibility.Visible;
                else if (b == "false")
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
