using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteSitesModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_culturalSites_CulturalSiteId1",
                table: "favoriteSites");

            migrationBuilder.DropIndex(
                name: "IX_favoriteSites_CulturalSiteId1",
                table: "favoriteSites");

            migrationBuilder.DropColumn(
                name: "CulturalSiteId1",
                table: "favoriteSites");

            migrationBuilder.AddColumn<string>(
                name: "culturalSiteModelCulturalSiteId",
                table: "favoriteSites",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_favoriteSites_culturalSiteModelCulturalSiteId",
                table: "favoriteSites",
                column: "culturalSiteModelCulturalSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_culturalSites_culturalSiteModelCulturalSiteId",
                table: "favoriteSites",
                column: "culturalSiteModelCulturalSiteId",
                principalTable: "culturalSites",
                principalColumn: "CulturalSiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_culturalSites_culturalSiteModelCulturalSiteId",
                table: "favoriteSites");

            migrationBuilder.DropIndex(
                name: "IX_favoriteSites_culturalSiteModelCulturalSiteId",
                table: "favoriteSites");

            migrationBuilder.DropColumn(
                name: "culturalSiteModelCulturalSiteId",
                table: "favoriteSites");

            migrationBuilder.AddColumn<string>(
                name: "CulturalSiteId1",
                table: "favoriteSites",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_favoriteSites_CulturalSiteId1",
                table: "favoriteSites",
                column: "CulturalSiteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_culturalSites_CulturalSiteId1",
                table: "favoriteSites",
                column: "CulturalSiteId1",
                principalTable: "culturalSites",
                principalColumn: "CulturalSiteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
