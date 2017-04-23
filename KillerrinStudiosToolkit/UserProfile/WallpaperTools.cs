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

namespace KillerrinStudiosToolkit.UserProfile
{
    public class WallpaperTools
    {
        public static WallpaperTools Instance { get; } = new WallpaperTools();

        public static StorageFolder WallpaperImagesFolder;
        public const string WallpaperImagesFolderName = "WallpaperImages";

        public bool IsSupported { get { return UserProfilePersonalizationSettings.IsSupported(); } }

        public WallpaperTools()
        {
            Debug.WriteLine($"{nameof(WallpaperTools)} - IsSupported: {IsSupported}");
            SetupFolder();
        }

        private async void SetupFolder()
        {
            WallpaperImagesFolder  = await StorageTask.Instance.CreateFolder(StorageTask.LocalFolder, WallpaperTools.WallpaperImagesFolderName, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<bool> DeleteAllImagesInWallpaperFolder()
        {
            return await StorageTask.Instance.DeleteAllFilesInFolder(WallpaperTools.WallpaperImagesFolder);
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

        public async Task<bool> SetWallpaperImage(Uri imageToSet, string id)
        {
            StorageFile file = await GetStorageFile(WallpaperTools.WallpaperImagesFolder, imageToSet, id);

            if (file == null) return false;
            return await SetWallpaperImage(file);
        }

        public async Task<bool> SetWallpaperImage(StorageFile imageFile)
        {
            if (imageFile == null) return false;

            try
            {
                bool success = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(imageFile);
                return success;
            }
            catch (Exception e)
            {
                DebugTools.PrintOutException(e, "Wallpaper: Set Image");
                return false;
            }
        }
    }
}
