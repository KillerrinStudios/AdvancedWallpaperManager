using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.Repositories
{
    public class WallpaperManagerContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=wallpaperManager.db");
        }

        // Create the DbSets
        public DbSet<WallpaperTheme> Themes { get; set; }
        public DbSet<WallpaperDirectory> Directories { get; set; }
        public DbSet<FileAccessToken> AccessTokens { get; set; }
        public DbSet<FileDiscoveryCache> FileDiscoveryCache { get; set; }

        // Create the Model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
