using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddPotSnapshotIsAutomatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutomatic",
                table: "PotSnapshots",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutomatic",
                table: "PotSnapshots");
        }
    }
}
