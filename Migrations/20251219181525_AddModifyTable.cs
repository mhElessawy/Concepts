using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddModifyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Vender",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterIdd",
                table: "Vender",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vender",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Deff_SubCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Deff_Department",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Deff_Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Def_UOM",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Def_SubUOM",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Vender");

            migrationBuilder.DropColumn(
                name: "CostCenterIdd",
                table: "Vender");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vender");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Deff_SubCategory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Deff_Department");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Deff_Category");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Def_UOM");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Def_SubUOM");
        }
    }
}
