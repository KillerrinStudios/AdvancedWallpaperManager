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
    partial class WallpaperManagerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("WallpaperManager.Models.FileAccessToken", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<int>("AccessTokenType");

                    b.Property<string>("Path");

                    b.HasKey("ID");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("WallpaperManager.Models.WallpaperDirectory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FileAccessTokenID");

                    b.Property<bool>("IncludeSubdirectories");

                    b.Property<bool>("IsExcluded");

                    b.Property<string>("Path");

                    b.Property<int>("StorageLocation");

                    b.Property<int>("WallpaperThemeID");

                    b.HasKey("ID");

                    b.HasIndex("FileAccessTokenID");

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
                    b.HasOne("WallpaperManager.Models.FileAccessToken", "AccessToken")
                        .WithMany()
                        .HasForeignKey("FileAccessTokenID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WallpaperManager.Models.WallpaperTheme", "Theme")
                        .WithMany("Directories")
                        .HasForeignKey("WallpaperThemeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
