﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedWallpaperManager.Models.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccessToken = table.Column<string>(nullable: true),
                    AccessTokenType = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCacheDiscovered = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateLastModified = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    WallpaperChangeFrequency = table.Column<TimeSpan>(nullable: false),
                    WallpaperSelectionMethod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FileDiscoveryCache",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateDiscovered = table.Column<DateTime>(nullable: false),
                    FileAccessTokenID = table.Column<int>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    FolderPath = table.Column<string>(nullable: true),
                    StorageLocation = table.Column<int>(nullable: false),
                    WallpaperThemeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDiscoveryCache", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FileDiscoveryCache_AccessTokens_FileAccessTokenID",
                        column: x => x.FileAccessTokenID,
                        principalTable: "AccessTokens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileDiscoveryCache_Themes_WallpaperThemeID",
                        column: x => x.WallpaperThemeID,
                        principalTable: "Themes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Directories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileAccessTokenID = table.Column<int>(nullable: false),
                    IncludeSubdirectories = table.Column<bool>(nullable: false),
                    IsExcluded = table.Column<bool>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    StorageLocation = table.Column<int>(nullable: false),
                    WallpaperThemeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Directories_AccessTokens_FileAccessTokenID",
                        column: x => x.FileAccessTokenID,
                        principalTable: "AccessTokens",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Directories_Themes_WallpaperThemeID",
                        column: x => x.WallpaperThemeID,
                        principalTable: "Themes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileDiscoveryCache_FileAccessTokenID",
                table: "FileDiscoveryCache",
                column: "FileAccessTokenID");

            migrationBuilder.CreateIndex(
                name: "IX_FileDiscoveryCache_WallpaperThemeID",
                table: "FileDiscoveryCache",
                column: "WallpaperThemeID");

            migrationBuilder.CreateIndex(
                name: "IX_Directories_FileAccessTokenID",
                table: "Directories",
                column: "FileAccessTokenID");

            migrationBuilder.CreateIndex(
                name: "IX_Directories_WallpaperThemeID",
                table: "Directories",
                column: "WallpaperThemeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDiscoveryCache");

            migrationBuilder.DropTable(
                name: "Directories");

            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
