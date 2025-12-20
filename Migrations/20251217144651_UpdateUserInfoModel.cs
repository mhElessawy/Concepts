using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInfoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseDefaultStatus",
                table: "UserInfo");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PurchaseOrderAuthorise",
                table: "UserInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderAuthorise",
                table: "UserInfo");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseDefaultStatus",
                table: "UserInfo",
                type: "int",
                nullable: true);
        }
    }
}
