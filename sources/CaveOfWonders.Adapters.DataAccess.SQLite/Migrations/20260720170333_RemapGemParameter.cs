using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class RemapGemParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GemParameters",
                table: "GemParameters");

            migrationBuilder.DropIndex(
                name: "IX_GemParameters_GemId",
                table: "GemParameters");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GemParameters");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "GemParameters",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GemParameters",
                table: "GemParameters",
                columns: new[] { "GemId", "Key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GemParameters",
                table: "GemParameters");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "GemParameters",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GemParameters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GemParameters",
                table: "GemParameters",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GemParameters_GemId",
                table: "GemParameters",
                column: "GemId");
        }
    }
}
