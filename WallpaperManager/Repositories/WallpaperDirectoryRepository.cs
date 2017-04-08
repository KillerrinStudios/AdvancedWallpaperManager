using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.Repositories
{
    public class WallpaperDirectoryRepository : RepositoryBase<WallpaperDirectory>
    {
        public WallpaperDirectoryRepository(WallpaperManagerContext context)
            : base(context)
        {
        }
    }
}
