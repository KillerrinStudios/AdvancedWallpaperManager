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
using WallpaperManager.Models.Enums;

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
        private ImageSelectionMethod m_wallpaperSelectionMethod = ImageSelectionMethod.Random;
        public ImageSelectionMethod WallpaperSelectionMethod
        {
            get { return m_wallpaperSelectionMethod; }
            set
            {
                m_wallpaperSelectionMethod = value;
                RaisePropertyChanged(nameof(WallpaperSelectionMethod));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private TimeSpan m_wallpaperChangeFrequency = TimeSpan.FromMinutes(15);
        public TimeSpan WallpaperChangeFrequency
        {
            get { return m_wallpaperChangeFrequency; }
            set
            {
                m_wallpaperChangeFrequency = value;
                RaisePropertyChanged(nameof(WallpaperChangeFrequency));
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
        private DateTime m_dateCacheDiscovered;
        public DateTime DateCacheDiscovered
        {
            get { return m_dateCacheDiscovered; }
            set
            {
                m_dateCacheDiscovered = value;
                RaisePropertyChanged(nameof(DateCacheDiscovered));
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
                    var files = repo.GetAllQuery()
                        .Where(x => x.WallpaperThemeID == ID)
                        .OrderBy(x => x.FolderPath)
                        .ThenBy(x => x.FilePath)
                        .ToList();

                    if (files.Count == 0) return "";

                    var rnd = RandomHelper.Random.Next(0, files.Count);
                    var file = files[rnd].FilePath;
                    return file;
                }
            }
        }

        [JsonIgnore]
        [NotMapped]
        public string FirstImageFromCache
        {
            get
            {
                using (var context = new WallpaperManagerContext())
                {
                    var repo = new GenericRepository<FileDiscoveryCache>(context);
                    var files = repo.GetAllQuery()
                        .Where(x => x.WallpaperThemeID == ID)
                        .OrderBy(x => x.FolderPath)
                        .ThenBy(x => x.FilePath)
                        .ToList();

                    if (files.Count == 0) return "";

                    var file = files[0].FilePath;
                    return file;
                }
            }
        }
    }
}
