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

namespace WallpaperManager.Models
{
    public class WallpaperDirectory : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(WallpaperThemeID))]
        public WallpaperTheme Theme { get; set; }
        public int WallpaperThemeID { get; set; }

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
        private string m_path = "";
        public string Path
        {
            get { return m_path; }
            set
            {
                m_path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private bool m_includeSubdirectories = true;
        public bool IncludeSubdirectories
        {
            get { return m_includeSubdirectories; }
            set
            {
                m_includeSubdirectories = value;
                RaisePropertyChanged(nameof(IncludeSubdirectories));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private bool m_isExcluded = false;
        public bool IsExcluded
        {
            get { return m_isExcluded; }
            set
            {
                m_isExcluded = value;
                RaisePropertyChanged(nameof(IsExcluded));
            }
        }

        [JsonIgnore]
        [NotMapped]
        public WallpaperDirectory Parent { get; set; }
    }

}
