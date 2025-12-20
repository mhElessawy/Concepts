using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreItemModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_Item_Deff_City_CityId",
                table: "Store_Item");

            migrationBuilder.DropIndex(
                name: "IX_Store_Item_CityId",
                table: "Store_Item");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Store_Item");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Store_Item");

            migrationBuilder.RenameColumn(
                name: "SubdueVOMMaxcode",
                table: "Store_Item",
                newName: "PackSize");

            migrationBuilder.RenameColumn(
                name: "LiveDRMMaxcode",
                table: "Store_Item",
                newName: "CountryId");

            migrationBuilder.AddColumn<string>(
                name: "ShortItemName",
                table: "Store_Item",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Store_Item_CountryId",
                table: "Store_Item",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Item_Deff_Country_CountryId",
                table: "Store_Item",
                column: "CountryId",
                principalTable: "Deff_Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_Item_Deff_Country_CountryId",
                table: "Store_Item");

            migrationBuilder.DropIndex(
                name: "IX_Store_Item_CountryId",
                table: "Store_Item");

            migrationBuilder.DropColumn(
                name: "ShortItemName",
                table: "Store_Item");

            migrationBuilder.RenameColumn(
                name: "PackSize",
                table: "Store_Item",
                newName: "SubdueVOMMaxcode");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Store_Item",
                newName: "LiveDRMMaxcode");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Store_Item",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Store_Item",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_Item_CityId",
                table: "Store_Item",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Item_Deff_City_CityId",
                table: "Store_Item",
                column: "CityId",
                principalTable: "Deff_City",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
