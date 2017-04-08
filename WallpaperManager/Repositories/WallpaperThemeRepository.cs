using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.Repositories
{
    public class WallpaperThemeRepository : RepositoryBase<WallpaperTheme>
    {
        public WallpaperThemeRepository(WallpaperManagerContext context)
            : base(context)
        {
        }
    }
}
