using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class CostCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deff_CostCenter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Deff_CostCenter",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ParentCostCenterId",
                table: "Deff_CostCenter",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Deff_CostCenter",
                type: "nvarchar(max)",
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

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Deff_CostCenter");

            migrationBuilder.DropColumn(
                name: "ParentCostCenterId",
                table: "Deff_CostCenter");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "Deff_CostCenter");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deff_CostCenter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
