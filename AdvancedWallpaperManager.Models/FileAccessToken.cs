using KillerrinStudiosToolkit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWallpaperManager.Models.Enums;

namespace AdvancedWallpaperManager.Models
{
    public class FileAccessToken : ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

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
        private string m_accessToken = "";

        [DataType(DataType.Text)]
        public string AccessToken
        {
            get { return m_accessToken; }
            set
            {
                m_accessToken = value;
                RaisePropertyChanged(nameof(AccessToken));
            }
        }

        [JsonIgnore]
        [NotMapped]
        private FileAccessTokenType m_accessTokenType;
        public FileAccessTokenType AccessTokenType
        {
            get { return m_accessTokenType; }
            set
            {
                m_accessTokenType = value;
                RaisePropertyChanged(nameof(AccessTokenType));
            }
        }

    }
}
