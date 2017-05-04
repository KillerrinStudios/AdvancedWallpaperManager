using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedWallpaperManager.Models;

namespace AdvancedWallpaperManager.DAL.Repositories
{
    public class FileDiscoveryCacheRepository : RepositoryBase<FileDiscoveryCache>
    {
        public FileDiscoveryCacheRepository(WallpaperManagerContext context)
            : base(context)
        {

        }
    }
}
