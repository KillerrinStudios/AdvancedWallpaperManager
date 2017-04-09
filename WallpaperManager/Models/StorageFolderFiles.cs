using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
}
