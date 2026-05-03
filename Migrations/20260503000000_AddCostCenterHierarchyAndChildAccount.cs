using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddCostCenterHierarchyAndChildAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ParentId and Target to Deff_CostCenter
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Deff_CostCenter",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Target",
                table: "Deff_CostCenter",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deff_CostCenter_ParentId",
                table: "Deff_CostCenter",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deff_CostCenter_Deff_CostCenter_ParentId",
                table: "Deff_CostCenter",
                column: "ParentId",
                principalTable: "Deff_CostCenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Create Deff_ChildAccount table
            migrationBuilder.CreateTable(
                name: "Deff_ChildAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentAccountId = table.Column<int>(type: "int", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deff_ChildAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deff_ChildAccount_Deff_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "Deff_CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Deff_ChildAccount_Deff_ChildAccount_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "Deff_ChildAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deff_ChildAccount_AccountCode",
                table: "Deff_ChildAccount",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deff_ChildAccount_CostCenterId",
                table: "Deff_ChildAccount",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Deff_ChildAccount_ParentAccountId",
                table: "Deff_ChildAccount",
                column: "ParentAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Deff_ChildAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Deff_CostCenter_Deff_CostCenter_ParentId",
                table: "Deff_CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_Deff_CostCenter_ParentId",
                table: "Deff_CostCenter");

            migrationBuilder.DropColumn(name: "ParentId", table: "Deff_CostCenter");
            migrationBuilder.DropColumn(name: "Target", table: "Deff_CostCenter");
        }
    }
}
