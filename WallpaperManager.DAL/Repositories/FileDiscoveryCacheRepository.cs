using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.DAL.Repositories
{
    public class FileDiscoveryCacheRepository : RepositoryBase<FileDiscoveryCache>
    {
        public FileDiscoveryCacheRepository(WallpaperManagerContext context)
            : base(context)
        {

        }
    }
}
