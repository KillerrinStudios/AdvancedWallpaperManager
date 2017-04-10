using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WallpaperManager.Repositories;
using WallpaperManager.Models.Enums;

namespace WallpaperManager.Migrations
{
    [DbContext(typeof(WallpaperManagerContext))]
    [Migration("20170409093944_AddedFutureAccessTokens")]
    partial class AddedFutureAccessTokens
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("WallpaperManager.Models.WallpaperDirectory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FutureAccessToken");

                    b.Property<bool>("IncludeSubdirectories");

                    b.Property<bool>("IsExcluded");

                    b.Property<string>("Path");

                    b.Property<int>("StorageLocation");

                    b.Property<int>("WallpaperThemeID");

                    b.HasKey("ID");

                    b.HasIndex("WallpaperThemeID");

                    b.ToTable("Directories");
                });

            modelBuilder.Entity("WallpaperManager.Models.WallpaperTheme", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastModified");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("WallpaperManager.Models.WallpaperDirectory", b =>
                {
                    b.HasOne("WallpaperManager.Models.WallpaperTheme", "Theme")
                        .WithMany("Directories")
                        .HasForeignKey("WallpaperThemeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
