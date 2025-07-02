using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteSitesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteSiteModel_culturalSites_CulturalSiteId1",
                table: "FavoriteSiteModel");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteSiteModel_profiles_userEmail",
                table: "FavoriteSiteModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteSiteModel",
                table: "FavoriteSiteModel");

            migrationBuilder.RenameTable(
                name: "FavoriteSiteModel",
                newName: "favoriteSites");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteSiteModel_userEmail",
                table: "favoriteSites",
                newName: "IX_favoriteSites_userEmail");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteSiteModel_CulturalSiteId1",
                table: "favoriteSites",
                newName: "IX_favoriteSites_CulturalSiteId1");

            migrationBuilder.AlterColumn<string>(
                name: "userEmail",
                table: "favoriteSites",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_favoriteSites",
                table: "favoriteSites",
                column: "FavoriteSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_culturalSites_CulturalSiteId1",
                table: "favoriteSites",
                column: "CulturalSiteId1",
                principalTable: "culturalSites",
                principalColumn: "CulturalSiteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_profiles_userEmail",
                table: "favoriteSites",
                column: "userEmail",
                principalTable: "profiles",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_culturalSites_CulturalSiteId1",
                table: "favoriteSites");

            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_profiles_userEmail",
                table: "favoriteSites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_favoriteSites",
                table: "favoriteSites");

            migrationBuilder.RenameTable(
                name: "favoriteSites",
                newName: "FavoriteSiteModel");

            migrationBuilder.RenameIndex(
                name: "IX_favoriteSites_userEmail",
                table: "FavoriteSiteModel",
                newName: "IX_FavoriteSiteModel_userEmail");

            migrationBuilder.RenameIndex(
                name: "IX_favoriteSites_CulturalSiteId1",
                table: "FavoriteSiteModel",
                newName: "IX_FavoriteSiteModel_CulturalSiteId1");

            migrationBuilder.AlterColumn<string>(
                name: "userEmail",
                table: "FavoriteSiteModel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteSiteModel",
                table: "FavoriteSiteModel",
                column: "FavoriteSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteSiteModel_culturalSites_CulturalSiteId1",
                table: "FavoriteSiteModel",
                column: "CulturalSiteId1",
                principalTable: "culturalSites",
                principalColumn: "CulturalSiteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteSiteModel_profiles_userEmail",
                table: "FavoriteSiteModel",
                column: "userEmail",
                principalTable: "profiles",
                principalColumn: "Email");
        }
    }
}
