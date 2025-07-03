using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteSiteModel",
                columns: table => new
                {
                    FavoriteSiteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    userEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CulturalSiteId = table.Column<int>(type: "int", nullable: false),
                    CulturalSiteId1 = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteSiteModel", x => x.FavoriteSiteId);
                    table.ForeignKey(
                        name: "FK_FavoriteSiteModel_culturalSites_CulturalSiteId1",
                        column: x => x.CulturalSiteId1,
                        principalTable: "culturalSites",
                        principalColumn: "CulturalSiteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteSiteModel_profiles_userEmail",
                        column: x => x.userEmail,
                        principalTable: "profiles",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSiteModel_CulturalSiteId1",
                table: "FavoriteSiteModel",
                column: "CulturalSiteId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSiteModel_userEmail",
                table: "FavoriteSiteModel",
                column: "userEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteSiteModel");
        }
    }
}
