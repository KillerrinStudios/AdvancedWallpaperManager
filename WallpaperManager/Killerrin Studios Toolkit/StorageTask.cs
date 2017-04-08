using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace Killerrin_Studios_Toolkit
{
    public enum StorageLocationPrefix
    {
        None,
        Solution,
        LocalFolder,
        RoamingFolder,
        TempFolder
    }

    public class StorageTask
    {
        public static StorageTask Instance { get; } = new StorageTask();

        #region Properties
        #region Prefix's and FileNames
        public const string SolutionPrefix = "ms-appx:///";
        public const string LocalFolderPrefix = "ms-appdata:///local/";
        public const string RoamingFolderPrefix = "ms-appdata:///roaming/";
        public const string TempFolderPrefix = "ms-appdata:///temp/";

        public static Uri CreateUri(StorageLocationPrefix prefix, string path)
        {
            switch (prefix)
            {
                case StorageLocationPrefix.Solution: return new Uri(SolutionPrefix + path, UriKind.Absolute);
                case StorageLocationPrefix.LocalFolder: return new Uri(LocalFolderPrefix + path, UriKind.Absolute);
                case StorageLocationPrefix.RoamingFolder: return new Uri(RoamingFolderPrefix + path, UriKind.Absolute);
                case StorageLocationPrefix.TempFolder: return new Uri(TempFolderPrefix + path, UriKind.Absolute);
                case StorageLocationPrefix.None: 
                default: return new Uri(path, UriKind.RelativeOrAbsolute);
            }
        }
        public static Uri GetPath(IStorageItem item)
        {
            return new Uri(item.Path, UriKind.Absolute);
        }
        #endregion

        #region Storage Folders
        public static StorageFolder PackageFolder { get { return Windows.ApplicationModel.Package.Current.InstalledLocation; } }

        public static StorageFolder LocalCacheFolder { get { return ApplicationData.Current.LocalCacheFolder; } }
        public static StorageFolder LocalFolder { get { return ApplicationData.Current.LocalFolder; } }
        public static StorageFolder SharedLocalFolder { get { return ApplicationData.Current.SharedLocalFolder; } }

        public static StorageFolder TemporaryFolder { get { return ApplicationData.Current.TemporaryFolder; } }

        public static StorageFolder RoamingFolder { get { return ApplicationData.Current.RoamingFolder; } }
        public static ulong RoamingStorageQuota { get { return ApplicationData.Current.RoamingStorageQuota; } }
        #endregion

        #region Settings Data Containers
        public static ApplicationDataContainer LocalSettings { get { return ApplicationData.Current.LocalSettings; } }
        public static ApplicationDataContainer RoamingSettings { get { return ApplicationData.Current.RoamingSettings; } }
        #endregion
        #endregion

        public StorageTask()
        {
        }

        public static PasswordVault GetPasswordVault()
        {
            PasswordVault vault = new PasswordVault();
            return vault;
        }

        #region Publisher Folder
        public StorageFolder GetPublisherCacheFolder(string folderName)
        {
            return ApplicationData.Current.GetPublisherCacheFolder(folderName);
        }
        public async Task ClearPublisherCacheFolderAsync(string folderName)
        {
            await ApplicationData.Current.ClearPublisherCacheFolderAsync(folderName);
        }
        #endregion

        #region Get Files/Folders
        #region Files
        public async Task<StorageFile> GetFileFromPath(Uri uri)
        {
            return await StorageFile.GetFileFromPathAsync(uri.OriginalString);
        }
        public async Task<StorageFile> GetFile(StorageFolder folder, string fileName)
        {
            return await folder.GetFileAsync(fileName);
        }
        public async Task<IReadOnlyList<StorageFile>> GetAllFilesInFolder(StorageFolder folder)
        {
            return await folder.GetFilesAsync();
        }
        #endregion

        #region Folders
        public async Task<StorageFolder> GetFolderFromPath(Uri uri)
        {
            return await StorageFolder.GetFolderFromPathAsync(uri.OriginalString);
        }
        public async Task<StorageFolder> GetFolder(StorageFolder folder, string folderName)
        {
            return await folder.GetFolderAsync(folderName);
        }
        public async Task<IReadOnlyList<StorageFolder>> GetAllFoldersInFolder(StorageFolder folder)
        {
            return await folder.GetFoldersAsync();
        }
        #endregion

        public async Task<IReadOnlyList<IStorageItem>> GetAllItemsInFolder(StorageFolder folder, CommonFileQuery query)
        {
            return await folder.GetItemsAsync();
        }
        #endregion

        #region Create/Read
        #region Create
        public async Task<StorageFolder> CreateFolder(StorageFolder folder, string folderName, CreationCollisionOption colisionOptions)
        {
            StorageFolder storageFolder = await folder.CreateFolderAsync(folderName, colisionOptions);
            return storageFolder;
        }

        public async Task<bool> CreateFile(StorageFolder folder, string fileName, IBuffer buffer)
        {
            Debug.WriteLine("Creating File: " + fileName);
            CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting;

            StorageFile file = await folder.CreateFileAsync(fileName, collisionOption);
            await FileIO.WriteBufferAsync(file, buffer);
            Debug.WriteLine("File Path: " + GetPath(file));

            return true;
        }
        public async Task<bool> CreateFile(StorageFolder folder, string fileName, byte[] bytes)
        {
            Debug.WriteLine("Creating File: " + fileName);
            CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting;

            StorageFile file = await folder.CreateFileAsync(fileName, collisionOption);
            await FileIO.WriteBytesAsync(file, bytes);
            Debug.WriteLine("File Path: " + GetPath(file));

            return true;
        }
        public async Task<bool> CreateFile(StorageFolder folder, string fileName, string content)
        {
            Debug.WriteLine("Creating File: " + fileName);
            CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting;

            StorageFile file = await folder.CreateFileAsync(fileName, collisionOption);
            await FileIO.WriteTextAsync(file, content);
            Debug.WriteLine("File Path: " + GetPath(file));

            return true;
        }
        #endregion

        #region Save from Server
        public async Task<bool> SaveFileFromServer(StorageFolder folder, string fileName, Uri serverURI)
        {
            //if (folder == null) return false;
            //if (string.IsNullOrWhiteSpace(fileName)) return false;
            //if (serverURI == null) return false;

            try
            {
                Debug.WriteLine("Opening Client");
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

                Debug.WriteLine("Getting Image Bytes");
                byte[] data = await client.GetByteArrayAsync(serverURI);
                return await CreateFile(folder, fileName, data);
            }
            catch (Exception) { }

            return false;
        }
        #endregion

        #region Read
        public async Task<IBuffer> ReadFileBuffer(StorageFile file)
        {
            if (file == null) return null;
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            return buffer;
        }
        public async Task<string> ReadFileString(StorageFile file)
        {
            if (file == null) return null;
            return await FileIO.ReadTextAsync(file);
        }
        public async Task<byte[]> ReadFileBytes(StorageFile file)
        {
            if (file == null) return null;
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);

            byte[] bytes = new byte[buffer.Length];
            reader.ReadBytes(bytes);
            return bytes;
        }
        #endregion
        #endregion

        #region Move/Copy
        public async Task<StorageFile> Copy (StorageFile item, StorageFolder destination)
        {
            return await item.CopyAsync(destination);
        }
        public async Task<bool> Move(StorageFile item, StorageFolder destination)
        {
            await item.MoveAsync(destination);
            return true;
        }
        #endregion

        public async Task<IStorageItem> DoesItemExist(StorageFolder folder, string name)
        {
            var storageItem = await folder.TryGetItemAsync(name);
            return storageItem;
        }

        #region Delete
        public async Task<bool> DeleteItem(IStorageItem item, StorageDeleteOption deletionOption)
        {
            await item.DeleteAsync(deletionOption);
            return true;
        }

        public async Task<bool> DeleteAllFilesInFolder(StorageFolder folder)
        {
            var files = await GetAllFilesInFolder(folder);
            foreach (var file in files)
            {
                await StorageTask.Instance.DeleteItem(file, StorageDeleteOption.Default);
            }
            return true;
        }
        #endregion

        #region Converters
        public static StorageFile IStorageItemToStorageFile(IStorageItem item)
        {
            if (item is StorageFile) return (StorageFile)item;
            return null;
        }
        public static StorageFolder IStorageItemToStorageFolder(IStorageItem item)
        {
            if (item is StorageFolder) return (StorageFolder)item;
            return null;
        }

        public static IStorageItem StorageFileToIStorageItem(StorageFile file)
        {
            return (IStorageItem)file;
        }
        public static IStorageItem StorageFolderToIStorageItem(StorageFolder folder)
        {
            return (IStorageItem)folder;
        }
        #endregion
    }
}
