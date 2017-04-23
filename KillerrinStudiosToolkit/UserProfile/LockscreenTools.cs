using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace KillerrinStudiosToolkit.UserProfile
{
    public class LockscreenTools
    {
        public static LockscreenTools Instance { get; } = new LockscreenTools();

        public static StorageFolder LockscreenImagesFolder;
        public const string LockscreenImagesFolderName = "LockscreenImages";

        public bool IsSupported { get { return UserProfilePersonalizationSettings.IsSupported(); } }

        public LockscreenTools()
        {
            Debug.WriteLine($"{nameof(LockscreenTools)} - IsSupported: {IsSupported}");
            SetupFolder();
        }

        private async void SetupFolder()
        {
            LockscreenImagesFolder = await StorageTask.Instance.CreateFolder(StorageTask.LocalFolder, LockscreenTools.LockscreenImagesFolderName, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<bool> DeleteAllImagesInLockscreenFolder()
        {
            return await StorageTask.Instance.DeleteAllFilesInFolder(LockscreenTools.LockscreenImagesFolder);
        }

        /// <summary>
        /// Sets the Lockscreen Image using a file within the Application Storage
        /// </summary>
        /// <param name="localStorageFile">A file within the Application Storage</param>
        /// <returns>If the Lockscreen Image was successfully Set</returns>
        public async Task<bool> SetLockscreenImage(StorageFile localStorageFile)
        {
            if (localStorageFile == null) return false;
            if (!IsSupported) return false;

            try
            {
                bool success = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(localStorageFile);
                return success;
            }
            catch (Exception e)
            {
                DebugTools.PrintOutException(e, "Lockscreen: Set Image");
                return false;
            }
        }

        /// <summary>
        /// Downloads and Sets the Lockscreen Image using a file from the Internet
        /// </summary>
        /// <param name="internetImageUrl">The http url leading to the file on the Internet</param>
        /// <param name="filename">The name of the newly created file</param>
        /// <returns>If the Lockscreen Image was successfully Set</returns>
        public async Task<bool> SetLockscreenImageFromInternet(Uri internetImageUrl, string filename)
        {
            StorageFile file = null;
            if (await StorageTask.Instance.SaveFileFromServer(LockscreenTools.LockscreenImagesFolder, filename, internetImageUrl))
                file = await StorageTask.Instance.GetFile(LockscreenTools.LockscreenImagesFolder, filename);

            if (file == null) return false;
            return await SetLockscreenImage(file);
        }

        /// <summary>
        /// Copies the file to the Application Storage and Sets the Lockscren Image
        /// </summary>
        /// <param name="path">The Full Path to a File within the Computers File System</param>
        /// <returns>If the Lockscreen Image was successfully Set</returns>
        public async Task<bool> SetLockscreenImageFromFileSystem(string path)
        {
            var tmpFile = await StorageFile.GetFileFromPathAsync(path);
            if (tmpFile == null) return false;

            var file = await tmpFile.CopyAsync(LockscreenTools.LockscreenImagesFolder, tmpFile.Name, NameCollisionOption.ReplaceExisting);
            if (file == null) return false;

            return await SetLockscreenImage(file);
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
