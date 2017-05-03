using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace KillerrinStudiosToolkit.UserProfile
{
    public abstract class PersonalizationManagerBase
    {
        public virtual bool IsSupported { get { return UserProfilePersonalizationSettings.IsSupported(); } }
        public bool IsSetupComplete { get { return ImagesFolder != null; } }

        public static int ClearImagesFolderEveryXImages = 0;
        public StorageFolder ImagesFolder { get; private set; }
        public string ImagesFolderName { get; private set; }

        public PersonalizationManagerBase(string folderName)
        {
            ImagesFolderName = folderName;
            InternalSetupImages();
        }

        private async void InternalSetupImages()
        {
            ImagesFolder = await SetupImageFolder();
        }

        protected abstract Task<StorageFolder> SetupImageFolder();
        public virtual async void DeleteFilesInFolders()
        {
            await DeleteFilesInFoldersAsync();
        }
        public virtual async Task DeleteFilesInFoldersAsync()
        {
            if (!IsSetupComplete) return;
            await StorageTask.Instance.DeleteAllFilesInFolder(ImagesFolder);
        }

        /// <summary>
        /// Sets the Image using a file within the Application Storage
        /// </summary>
        /// <param name="localStorageFile">A file within the Application Storage</param>
        /// <returns>If the Image was successfully Set</returns>
        public abstract Task<bool> SetImage(StorageFile localStorageFile);

        /// <summary>
        /// Downloads and Sets the Image using a file from the Internet
        /// </summary>
        /// <param name="internetImageUrl">The http url leading to the Image File on the Internet</param>
        /// <param name="filename">The name of the newly created file with file extension</param>
        /// <returns>If the Image was successfully Set</returns>
        public virtual async Task<bool> SetImageFromInternet(Uri internetImageUrl, string filename)
        {
            if (!IsSetupComplete) return false;

            if (ClearImagesFolderEveryXImages != 0)
            {
                int fileCount = await StorageTask.Instance.FileCount(ImagesFolder);
                if (fileCount > ClearImagesFolderEveryXImages)
                    await DeleteFilesInFoldersAsync();
            }

            StorageFile file = null;
            if (await StorageTask.Instance.SaveFileFromServer(ImagesFolder, filename, internetImageUrl))
                file = await StorageTask.Instance.GetFile(ImagesFolder, filename);

            return await SetImage(file);
        }

        /// <summary>
        /// Copies the file to the Application Storage and Sets the Image
        /// </summary>
        /// <param name="path">The Full Path to an Image File within the Computers File System</param>
        /// <returns>If the Image was successfully Set</returns>
        public virtual async Task<bool> SetImageFromFileSystem(string path)
        {
            if (!IsSetupComplete) return false;

            if (ClearImagesFolderEveryXImages != 0)
            {
                int fileCount = await StorageTask.Instance.FileCount(ImagesFolder);
                if (fileCount > ClearImagesFolderEveryXImages)
                    await DeleteFilesInFoldersAsync();
            }

            var tmpFile = await StorageFile.GetFileFromPathAsync(path);
            if (tmpFile == null) return false;

            var copiedFile = await tmpFile.CopyAsync(ImagesFolder, tmpFile.Name, NameCollisionOption.ReplaceExisting);

            var localStorageUri = StorageTask.CreateUri(StorageLocationPrefix.LocalFolder, $"{ImagesFolderName}/{copiedFile.Name}");
            var localStorageFile = await StorageFile.GetFileFromApplicationUriAsync(localStorageUri);

            return await SetImage(localStorageFile);
        }
    }
}
