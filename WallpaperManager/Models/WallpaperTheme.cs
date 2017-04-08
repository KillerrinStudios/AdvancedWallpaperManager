using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager.Models
{
    public class WallpaperTheme : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [JsonIgnore]
        [NotMapped]
        private string m_name = "";
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private DateTime m_dateCreated;
        public DateTime DateCreated
        {
            get { return m_dateCreated; }
            set
            {
                m_dateCreated = value;
                RaisePropertyChanged(nameof(DateCreated));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private DateTime m_dateLastModified;
        public DateTime DateLastModified
        {
            get { return m_dateLastModified; }
            set
            {
                m_dateLastModified = value;
                RaisePropertyChanged(nameof(DateLastModified));
            }
        }

        public ObservableCollection<WallpaperDirectory> Directories { get; set; } = new ObservableCollection<WallpaperDirectory>();

        [JsonIgnore]
        [NotMapped]
        public string RandomImageFromDirectory
        {
            get
            {
                if (Directories.Count == 0) return "";

                var rnd = App.Random.Next(0, Directories.Count);

                var file = Directories[rnd].Path;
                return file;
            }
        }
    }
}
