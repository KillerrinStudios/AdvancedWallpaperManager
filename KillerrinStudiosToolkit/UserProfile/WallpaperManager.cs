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
    public class WallpaperManager : PersonalizationManagerBase
    {
        public static WallpaperManager Instance { get; } = new WallpaperManager();
        public const string WallpaperImagesFolderName = "WallpaperImages";

        public WallpaperManager()
            : base(WallpaperImagesFolderName)
        {
            Debug.WriteLine($"{nameof(WallpaperManager)} - IsSupported: {IsSupported}");
        }

        protected override async Task<StorageFolder> SetupImageFolder()
        {
            return await StorageTask.Instance.CreateFolder(StorageTask.LocalFolder, WallpaperImagesFolderName, CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Sets the Wallpaper Image using a file within the Application Storage
        /// </summary>
        /// <param name="localStorageFile">A file within the Application Storage</param>
        /// <returns>If the Wallpaper Image was successfully Set</returns>
        public override async Task<bool> SetImage(StorageFile localStorageFile)
        {
            if (!IsSupported) return false;
            if (localStorageFile == null) return false;

            try
            {
                bool success = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(localStorageFile);
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
