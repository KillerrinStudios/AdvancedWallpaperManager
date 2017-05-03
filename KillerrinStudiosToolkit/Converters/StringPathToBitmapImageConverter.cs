using KillerrinStudiosToolkit.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace KillerrinStudiosToolkit.Converters
{
    public class StringPathToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string pathString = ((string)value);

            // Preform Early Exit
            if (string.IsNullOrWhiteSpace(pathString))
                return null;

            Debug.WriteLine($"{nameof(StringPathToBitmapImageConverter)} - Begin: {pathString}");

            var task = Task.Run(async () =>
            {
                try
                {
                    var file = await StorageTask.Instance.GetFileFromPath(new Uri(pathString));
                    var bitmapFile = await StorageTask.StorageFileToBitmapImage(file);
                    return bitmapFile;
                }
                catch (Exception ex)
                {
                    //DebugTools.PrintOutException(ex, "Error Converting Path");
                    return null;
                }
            });
            return new TaskCompletionNotifier<BitmapImage>(task);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
