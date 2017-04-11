using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.Repositories
{
    public class FileAccessTokenRepository : RepositoryBase<FileAccessToken>
    {
        public FileAccessTokenRepository(WallpaperManagerContext context)
            : base(context)
        {
        }
    }
}
