using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace KillerrinStudiosToolkit
{
    public class LockscreenTools
    {
        public static LockscreenTools Instance { get; } = new LockscreenTools();

        public static StorageFolder LockscreenImagesFolder;
        public const string LockscreenImagesFolderName = "LockscreenImages";

        protected static StorageFolder RootFolder { get { return StorageTask.LocalFolder; } }

        public LockscreenTools()
        {
            SetupFolder();
        }

        private async void SetupFolder()
        {
            LockscreenImagesFolder = await StorageTask.Instance.CreateFolder(RootFolder, LockscreenTools.LockscreenImagesFolderName, CreationCollisionOption.OpenIfExists);
        }


        public static async Task<bool> DeleteAllImagesInLockscreenFolder()
        {
            return await StorageTask.Instance.DeleteAllFilesInFolder(LockscreenTools.LockscreenImagesFolder);
        }

        private async Task<StorageFile> GetStorageFile(StorageFolder folder, Uri imageToSet, string id)
        {
            if (folder == null) return null;

            StorageFile file = null;
            if (imageToSet.OriginalString.Contains("http://") ||
                imageToSet.OriginalString.Contains("https://"))
            {
                string fileName = id + ".jpg";
                if (await StorageTask.Instance.SaveFileFromServer(folder, fileName, imageToSet))
                    file = await StorageTask.Instance.GetFile(folder, fileName);
            }
            else file = await StorageFile.GetFileFromPathAsync(imageToSet.OriginalString);

            return file;
        }

        public async Task<bool> SetLockscreenImage(Uri imageToSet, string id)
        {
            StorageFile file = await GetStorageFile(LockscreenTools.LockscreenImagesFolder, imageToSet, id);

            if (file == null) return false;
            return await SetLockscreenImage(file);
        }

        public async Task<bool> SetLockscreenImage(StorageFile imageFile)
        {
            if (imageFile == null) return false;

            try
            {
                bool success = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(imageFile);
                return success;
            }
            catch (Exception e)
            {
                DebugTools.PrintOutException(e, "Lockscreen: Set Image");
                return false;
            }
        }

        public static IRandomAccessStream GetLockscreenImageStream()
        {
            IRandomAccessStream imageStream = LockScreen.GetImageStream();
            return imageStream;
        }
        public static BitmapImage GetLockscreenBitMapImage()
        {
            IRandomAccessStream imageStream = GetLockscreenImageStream();
            if (imageStream != null)
            {
                BitmapImage lockscreenImage = new BitmapImage();
                lockscreenImage.SetSource(imageStream);
                return lockscreenImage;
            }
            return null;
        }
    }
}
