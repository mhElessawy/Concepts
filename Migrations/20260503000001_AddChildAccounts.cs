using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddChildAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Child_Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentAccountId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountEffect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NatureOfAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreatsAsBankAccount = table.Column<bool>(type: "bit", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    FixedCostCenter = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CivilId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceiptLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Child_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Child_Account_Main_Account_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "Main_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Child_Account_Deff_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "Deff_CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Child_Account_AccountNo",
                table: "Child_Account",
                column: "AccountNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Child_Account_ParentAccountId",
                table: "Child_Account",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Child_Account_CostCenterId",
                table: "Child_Account",
                column: "CostCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Child_Account");
        }
    }
}
