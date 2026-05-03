using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class UPdateVedon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LedgerAccountCode",
                table: "Vender",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LedgerAccountName",
                table: "Vender",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LedgerAccountCode",
                table: "Vender");

            migrationBuilder.DropColumn(
                name: "LedgerAccountName",
                table: "Vender");
        }
    }
}
