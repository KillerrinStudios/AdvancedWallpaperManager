using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WallpaperManager.Models.Migrations
{
    public partial class AddedFileDiscoveryCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileDiscoveryCache",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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

            migrationBuilder.CreateIndex(
                name: "IX_FileDiscoveryCache_FileAccessTokenID",
                table: "FileDiscoveryCache",
                column: "FileAccessTokenID");

            migrationBuilder.CreateIndex(
                name: "IX_FileDiscoveryCache_WallpaperThemeID",
                table: "FileDiscoveryCache",
                column: "WallpaperThemeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileDiscoveryCache");
        }
    }
}
