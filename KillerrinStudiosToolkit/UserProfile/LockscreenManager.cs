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
    public class LockscreenManager : PersonalizationManagerBase
    {
        public static LockscreenManager Instance { get; } = new LockscreenManager();
        public const string LockscreenImagesFolderName = "LockscreenImages";

        public LockscreenManager()
            : base(LockscreenImagesFolderName)
        {
            Debug.WriteLine($"{nameof(LockscreenManager)} - IsSupported: {IsSupported}");
        }

        protected override async Task<StorageFolder> SetupImageFolder()
        {
            return await StorageTask.Instance.CreateFolder(StorageTask.LocalFolder, LockscreenImagesFolderName, CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Sets the Lockscreen Image using a file within the Application Storage
        /// </summary>
        /// <param name="localStorageFile">A file within the Application Storage</param>
        /// <returns>If the Lockscreen Image was successfully Set</returns>
        public override async Task<bool> SetImage(StorageFile localStorageFile)
        {
            if (localStorageFile == null) return false;
            if (!IsSupported) return false;

            try
            {
                if (!await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(localStorageFile))
                {
                    await LockScreen.SetImageFileAsync(localStorageFile);
                }
            }
            catch (Exception e)
            {
                DebugTools.PrintOutException(e, "Lockscreen: Set Image");
                return false;
            }

            return true;
        }

        public static IRandomAccessStream GetLockscreenImageStream()
        {
            try
            {
                IRandomAccessStream imageStream = LockScreen.GetImageStream();
                return imageStream;
            }
            catch (Exception) { }
            return null;
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
