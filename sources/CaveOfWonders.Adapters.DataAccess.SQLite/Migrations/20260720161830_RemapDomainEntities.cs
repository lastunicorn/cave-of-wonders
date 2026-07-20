using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class RemapDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PotLabels",
                table: "PotLabels");

            migrationBuilder.DropIndex(
                name: "IX_PotLabels_PotId",
                table: "PotLabels");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PotLabels");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "PotLabels",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyPair",
                table: "ExchangeRates",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PotLabels",
                table: "PotLabels",
                columns: new[] { "PotId", "Label" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PotLabels",
                table: "PotLabels");

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "PotLabels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PotLabels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyPair",
                table: "ExchangeRates",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PotLabels",
                table: "PotLabels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PotLabels_PotId",
                table: "PotLabels",
                column: "PotId");
        }
    }
}
