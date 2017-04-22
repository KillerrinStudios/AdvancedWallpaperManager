using KillerrinStudiosToolkit.Helpers;
using KillerrinStudiosToolkit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.DAL.Repositories;

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

        [JsonIgnore]
        [NotMapped]
        public string RandomImageFromCache
        {
            get
            {
                using (var context = new WallpaperManagerContext())
                {
                    var repo = new GenericRepository<FileDiscoveryCache>(context);
                    var files = repo.GetAllQuery().Where(x => x.WallpaperThemeID == ID).ToList();

                    if (files.Count == 0) return "";

                    var rnd = RandomHelper.Random.Next(0, files.Count);
                    var file = files[rnd].FilePath;
                    return file;
                }
            }
        }

        [JsonIgnore]
        [NotMapped]
        public string FirstImageImageFromDirectories
        {
            get
            {
                using (var context = new WallpaperManagerContext())
                {
                    var repo = new GenericRepository<FileDiscoveryCache>(context);
                    var files = repo.GetAllQuery().Where(x => x.WallpaperThemeID == ID).ToList();

                    if (files.Count == 0) return "";

                    var file = files[0].FilePath;
                    return file;
                }
            }
        }
    }
}
