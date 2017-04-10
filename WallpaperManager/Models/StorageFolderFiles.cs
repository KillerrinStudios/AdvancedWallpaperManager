using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Threading;

namespace WallpaperManager.Models
{
    public class StorageFolderFiles : ModelBase
    {
        private StorageFolder m_folder;
        public StorageFolder Folder
        {
            get { return m_folder; }
            set
            {
                m_folder = value;
                RaisePropertyChanged(nameof(Folder));
            }
        }

        private List<StorageFile> m_files = new List<StorageFile>();
        public List<StorageFile> Files
        {
            get { return m_files; }
            set
            {
                m_files = value;
                RaisePropertyChanged(nameof(Files));
            }
        }
    }

    public class StorageFolderFilesSource : IIncrementalSource<StorageFolderFiles>
    {
        public readonly List<StorageFolderFiles> FolderFiles;

        public StorageFolderFilesSource()
        {
            // Creates an example collection.
            FolderFiles = new List<StorageFolderFiles>();
        }

        public async Task<IEnumerable<StorageFolderFiles>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in FolderFiles
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            return result;
        }
    }
}
