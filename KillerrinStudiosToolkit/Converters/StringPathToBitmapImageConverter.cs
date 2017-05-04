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
            string pathString = value as string;
            if (string.IsNullOrWhiteSpace(pathString))
                return null;

            try
            {
                //Debug.WriteLine($"{nameof(StringPathToBitmapImageConverter)} - Begin: {pathString}");
                Uri pathUri = new Uri(pathString);
                BitmapImage image = new BitmapImage(pathUri);
                image.ImageFailed += Image_ImageFailed;
                image.ImageOpened += Image_ImageOpened;
                return image;
            }
            catch (Exception) { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static void Image_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Debug.WriteLine("Image successfully loaded");
            try
            {
                BitmapImage image = (BitmapImage)sender;
            }
            catch (Exception) { }
        }
        private static async void Image_ImageFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine(e.ErrorMessage);

            try
            {
                BitmapImage image = (BitmapImage)sender;

                Uri pathUri = new Uri(image.UriSource.LocalPath);
                var file = await StorageTask.Instance.GetFileFromPath(pathUri);

                using (var fileStream = await StorageTask.Instance.OpenFileStream(file, Windows.Storage.FileAccessMode.Read))
                {
                    image.SetSource(fileStream);
                }
            }
            catch (Exception) { }
        }
    }
}
