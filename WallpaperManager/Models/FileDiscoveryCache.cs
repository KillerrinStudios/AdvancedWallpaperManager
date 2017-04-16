using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models.Enums;
using Windows.Storage;

namespace WallpaperManager.Models
{
    public class FileDiscoveryCache : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(WallpaperThemeID))]
        public WallpaperTheme Theme { get; set; }
        public int WallpaperThemeID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(FileAccessTokenID))]
        public FileAccessToken AccessToken { get; set; }
        public int FileAccessTokenID { get; set; }

        [JsonIgnore]
        [NotMapped]
        private StorageLocation m_storageLocation;
        public StorageLocation StorageLocation
        {
            get { return m_storageLocation; }
            set
            {
                m_storageLocation = value;
                RaisePropertyChanged(nameof(StorageLocation));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private string m_folderPath = "";
        public string FolderPath
        {
            get { return m_folderPath; }
            set
            {
                m_folderPath = value;
                RaisePropertyChanged(nameof(FolderPath));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private string m_filePath = "";
        public string FilePath
        {
            get { return m_filePath; }
            set
            {
                m_filePath = value;
                RaisePropertyChanged(nameof(FilePath));
            }
        }
    }

    public class GroupedFileCache : ModelBase
    {
        public string FolderPath { get; set; }
        public List<FileDiscoveryCache> Files { get; set; } = new List<FileDiscoveryCache>();
    }
}
