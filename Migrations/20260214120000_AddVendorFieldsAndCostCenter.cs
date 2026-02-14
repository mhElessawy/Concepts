using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorFieldsAndCostCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNo",
                table: "Vender",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentTerms",
                table: "Vender",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Deff_CostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CostCenterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deff_CostCenter", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vender_CostCenterId",
                table: "Vender",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Deff_CostCenter_CostCenterCode",
                table: "Deff_CostCenter",
                column: "CostCenterCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vender_Deff_CostCenter_CostCenterId",
                table: "Vender",
                column: "CostCenterId",
                principalTable: "Deff_CostCenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vender_Deff_CostCenter_CostCenterId",
                table: "Vender");

            migrationBuilder.DropIndex(
                name: "IX_Vender_CostCenterId",
                table: "Vender");

            migrationBuilder.DropTable(
                name: "Deff_CostCenter");

            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "Vender");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "Vender");
        }
    }
}
