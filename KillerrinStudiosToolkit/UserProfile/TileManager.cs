using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace KillerrinStudiosToolkit.UserProfile
{
    public class TileImages
    {
        public Uri Square30x30;
        public Uri Square44x44;
        public Uri Square70x70;
        public Uri Square71x71;
        public Uri Square150x150;
        public Uri Square310x310;
        public Uri Wide310x150;

        public TileImages(Uri setAll)
        {
            Square30x30     = setAll;
            Square44x44     = setAll;
            Square70x70     = setAll;
            Square71x71     = setAll;
            Square150x150   = setAll;
            Square310x310   = setAll;
            Wide310x150     = setAll;
        }
    }

    public class TileManager
    {
        public static TileManager Instance { get; } = new TileManager();
        protected const string TileImagesFolderName = "TileImages";
        protected static StorageFolder RootFolder { get { return StorageTask.LocalFolder; } }

        public TileManager()
        {
            CreateStorageFolders();
        }

        private async void CreateStorageFolders()
        {
            await StorageTask.Instance.CreateFolder(RootFolder, TileManager.TileImagesFolderName, CreationCollisionOption.OpenIfExists);
        }

        public async Task<SecondaryTile> CreateTile(string tileID, string displayName, string activationArguments, TileImages tileImages, bool roamingEnabled = false)
        {
            Uri tileImage = null;
            if (tileImages.Square310x310.OriginalString.Contains("http://") ||
                tileImages.Square310x310.OriginalString.Contains("https://"))
            {
                StorageFolder folder = await StorageTask.Instance.CreateFolder(RootFolder, TileManager.TileImagesFolderName, CreationCollisionOption.OpenIfExists);
                if (folder != null)
                {
                    string fileName = tileID + ".jpg";
                    if (await StorageTask.Instance.SaveFileFromServer(folder, fileName, tileImages.Square310x310))
                        tileImage = StorageTask.CreateUri(StorageLocationPrefix.LocalFolder, TileImagesFolderName + "/" + fileName);
                }
            }
            if (tileImage == null)
                tileImage = tileImages.Square310x310;

            SecondaryTile secondaryTile = new SecondaryTile(tileID, displayName, activationArguments, tileImage, TileSize.Square150x150);
            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;

            // Images
            //secondaryTile.VisualElements.Square30x30Logo = tileImage; // Deprecated in Windows 10
            //secondaryTile.VisualElements.Square70x70Logo = tileImage; // Deprecated in Windows 10
            secondaryTile.VisualElements.Square44x44Logo = tileImage;
            secondaryTile.VisualElements.Square71x71Logo = tileImage;
            secondaryTile.VisualElements.Square310x310Logo = tileImage;
            secondaryTile.VisualElements.Wide310x150Logo = tileImage;

            // Names
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;

            // Roaming
            secondaryTile.RoamingEnabled = roamingEnabled;

            return secondaryTile;
        }

        public async Task<bool> Pin(SecondaryTile tile, Placement placement, Rect elementRect)
        {
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
            {
                return await tile.RequestCreateForSelectionAsync(elementRect, placement);
            }
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons")))
            {
                // Since pinning a secondary tile on Windows Phone will exit the app and take you to the start screen, any code after 
                // RequestCreateForSelectionAsync or RequestCreateAsync is not guaranteed to run.
                return await tile.RequestCreateAsync();
            }

            return false;
        }
        public async Task<bool> UnPin(SecondaryTile tile, Rect elementRect)
        { 
            bool result =  await tile.RequestDeleteForSelectionAsync(elementRect, Windows.UI.Popups.Placement.Below);
            if (result)
            {
                try
                {
                    StorageFolder tileFolder = await StorageTask.Instance.GetFolder(RootFolder, TileManager.TileImagesFolderName);
                    StorageFile imageFile = await StorageTask.Instance.GetFile(tileFolder, tile.TileId + ".jpg");
                    await StorageTask.Instance.DeleteItem(StorageTask.StorageFileToIStorageItem(imageFile), StorageDeleteOption.PermanentDelete);
                }
                catch (Exception) { }
            }

            return result;
        }

        public bool IsPinned(string tileID) { return Exists(tileID); }

        #region Helpers
        public static SecondaryTile GetTile(string tileID)
        {
            return new SecondaryTile(tileID);
        }
        public static bool Exists(string tileID)
        {
            return SecondaryTile.Exists(tileID);
        }
        public static async Task<IReadOnlyList<SecondaryTile>> GetAllTiles()
        {
            return await Windows.UI.StartScreen.SecondaryTile.FindAllAsync();
        }
        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
        #endregion
    }
}
