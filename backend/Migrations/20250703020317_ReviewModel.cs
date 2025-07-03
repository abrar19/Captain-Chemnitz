using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ReviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileModelEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CulturalSiteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CulturalSiteModelCulturalSiteId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reviews_culturalSites_CulturalSiteModelCulturalSiteId",
                        column: x => x.CulturalSiteModelCulturalSiteId,
                        principalTable: "culturalSites",
                        principalColumn: "CulturalSiteId");
                    table.ForeignKey(
                        name: "FK_reviews_profiles_ProfileModelEmail",
                        column: x => x.ProfileModelEmail,
                        principalTable: "profiles",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_CulturalSiteModelCulturalSiteId",
                table: "reviews",
                column: "CulturalSiteModelCulturalSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_ProfileModelEmail",
                table: "reviews",
                column: "ProfileModelEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reviews");
        }
    }
}
