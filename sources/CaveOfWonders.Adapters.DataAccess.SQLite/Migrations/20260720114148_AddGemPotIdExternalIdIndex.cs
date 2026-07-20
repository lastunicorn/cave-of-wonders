using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddGemPotIdExternalIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Gems_PotId",
                table: "Gems");

            migrationBuilder.CreateIndex(
                name: "IX_Gems_PotId_ExternalId",
                table: "Gems",
                columns: new[] { "PotId", "ExternalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Gems_PotId_ExternalId",
                table: "Gems");

            migrationBuilder.CreateIndex(
                name: "IX_Gems_PotId",
                table: "Gems",
                column: "PotId");
        }
    }
}
