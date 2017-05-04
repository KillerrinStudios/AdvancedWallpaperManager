using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedWallpaperManager.Models;

namespace AdvancedWallpaperManager.DAL.Repositories
{
    public class WallpaperDirectoryRepository : RepositoryBase<WallpaperDirectory>
    {
        public WallpaperDirectoryRepository(WallpaperManagerContext context)
            : base(context)
        {
        }
    }
}
