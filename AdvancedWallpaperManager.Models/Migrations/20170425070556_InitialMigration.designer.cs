using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AdvancedWallpaperManager.DAL.Repositories;
using AdvancedWallpaperManager.Models.Enums;

namespace AdvancedWallpaperManager.Models.Migrations
{
    [DbContext(typeof(WallpaperManagerContext))]
    [Migration("20170425070556_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("AdvancedWallpaperManager.Models.FileAccessToken", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<int>("AccessTokenType");

                    b.Property<string>("Path");

                    b.HasKey("ID");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("AdvancedWallpaperManager.Models.FileDiscoveryCache", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateDiscovered");

                    b.Property<int>("FileAccessTokenID");

                    b.Property<string>("FilePath");

                    b.Property<string>("FolderPath");

                    b.Property<int>("StorageLocation");

                    b.Property<int>("WallpaperThemeID");

                    b.HasKey("ID");

                    b.HasIndex("FileAccessTokenID");

                    b.HasIndex("WallpaperThemeID");

                    b.ToTable("FileDiscoveryCache");
                });

            modelBuilder.Entity("AdvancedWallpaperManager.Models.WallpaperDirectory", b =>
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

            modelBuilder.Entity("AdvancedWallpaperManager.Models.WallpaperTheme", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCacheDiscovered");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastModified");

                    b.Property<string>("Name");

                    b.Property<TimeSpan>("WallpaperChangeFrequency");

                    b.Property<int>("WallpaperSelectionMethod");

                    b.HasKey("ID");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("AdvancedWallpaperManager.Models.FileDiscoveryCache", b =>
                {
                    b.HasOne("AdvancedWallpaperManager.Models.FileAccessToken", "AccessToken")
                        .WithMany()
                        .HasForeignKey("FileAccessTokenID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AdvancedWallpaperManager.Models.WallpaperTheme", "Theme")
                        .WithMany()
                        .HasForeignKey("WallpaperThemeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AdvancedWallpaperManager.Models.WallpaperDirectory", b =>
                {
                    b.HasOne("AdvancedWallpaperManager.Models.FileAccessToken", "AccessToken")
                        .WithMany()
                        .HasForeignKey("FileAccessTokenID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AdvancedWallpaperManager.Models.WallpaperTheme", "Theme")
                        .WithMany()
                        .HasForeignKey("WallpaperThemeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
