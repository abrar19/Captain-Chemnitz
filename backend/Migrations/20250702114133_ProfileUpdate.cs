using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ProfileUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_profiles_userEmail",
                table: "favoriteSites");

            migrationBuilder.RenameColumn(
                name: "userEmail",
                table: "favoriteSites",
                newName: "profileModelEmail");

            migrationBuilder.RenameIndex(
                name: "IX_favoriteSites_userEmail",
                table: "favoriteSites",
                newName: "IX_favoriteSites_profileModelEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_profiles_profileModelEmail",
                table: "favoriteSites",
                column: "profileModelEmail",
                principalTable: "profiles",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favoriteSites_profiles_profileModelEmail",
                table: "favoriteSites");

            migrationBuilder.RenameColumn(
                name: "profileModelEmail",
                table: "favoriteSites",
                newName: "userEmail");

            migrationBuilder.RenameIndex(
                name: "IX_favoriteSites_profileModelEmail",
                table: "favoriteSites",
                newName: "IX_favoriteSites_userEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_favoriteSites_profiles_userEmail",
                table: "favoriteSites",
                column: "userEmail",
                principalTable: "profiles",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
