﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WallpaperManager.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                string b = ((string)value);
                if (b == "true" || b == "True" || b == "TRUE")
                    return Visibility.Collapsed;
                else if (b == "false" || b == "False" || b == "FALSE")
                    return Visibility.Visible;
            }
            else if (value is bool)
            {
                bool b = ((bool)value);
                if (b)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility v = ((Visibility)value);

            if (v == Visibility.Visible) return false;
            else if (v == Visibility.Collapsed) return true;
            return true;
        }
    }
}
