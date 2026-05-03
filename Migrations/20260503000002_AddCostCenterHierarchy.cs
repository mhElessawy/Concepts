using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddCostCenterHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Deff_CostCenter",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Deff_CostCenter",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentCostCenterId",
                table: "Deff_CostCenter",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deff_CostCenter_ParentCostCenterId",
                table: "Deff_CostCenter",
                column: "ParentCostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deff_CostCenter_Deff_CostCenter_ParentCostCenterId",
                table: "Deff_CostCenter",
                column: "ParentCostCenterId",
                principalTable: "Deff_CostCenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deff_CostCenter_Deff_CostCenter_ParentCostCenterId",
                table: "Deff_CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_Deff_CostCenter_ParentCostCenterId",
                table: "Deff_CostCenter");

            migrationBuilder.DropColumn(name: "Date", table: "Deff_CostCenter");
            migrationBuilder.DropColumn(name: "Target", table: "Deff_CostCenter");
            migrationBuilder.DropColumn(name: "ParentCostCenterId", table: "Deff_CostCenter");
        }
    }
}
