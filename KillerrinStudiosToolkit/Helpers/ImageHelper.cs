using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace KillerrinStudiosToolkit.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage StreamToBitmapImage(IRandomAccessStream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        public static BitmapImage PathToBitmapImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));
            return bitmapImage;
        }

        public static async Task<BitmapImage> StorageFileToBitmapImage(Windows.Storage.StorageFile file)
        {
            return await StorageTask.StorageFileToBitmapImage(file);
        }

    }
}
