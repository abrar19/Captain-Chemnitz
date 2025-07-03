using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class CulturalSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "culturalSites",
                columns: table => new
                {
                    CulturalSiteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    Landuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Museum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tourism = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wheelchair = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wikidata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddrCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddrHousenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddrPostcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddrStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirConditioning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amenity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingLevels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cuisine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Delivery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DietHalal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DietKosher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DietVegan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DietVegetarian = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndoorSeating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Microbrewery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutdoorSeating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentCards = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentCreditCards = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDebitCards = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoofMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoofShape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Smoking = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Takeaway = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteMenu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_culturalSites", x => x.CulturalSiteId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "culturalSites");
        }
    }
}
