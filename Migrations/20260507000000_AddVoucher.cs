using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Voucher_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountingSettlement = table.Column<bool>(type: "bit", nullable: false),
                    SettlementYear = table.Column<int>(type: "int", nullable: false),
                    Statement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Posting = table.Column<bool>(type: "bit", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher_Header", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Voucher_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    ChildAccountId = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NatureOfAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    CostCenterName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Child_Account_ChildAccountId",
                        column: x => x.ChildAccountId,
                        principalTable: "Child_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Deff_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "Deff_CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Voucher_Header_VoucherHeaderId",
                        column: x => x.VoucherHeaderId,
                        principalTable: "Voucher_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Header_VoucherNo",
                table: "Voucher_Header",
                column: "VoucherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_ChildAccountId",
                table: "Voucher_Details",
                column: "ChildAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_CostCenterId",
                table: "Voucher_Details",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_VoucherHeaderId",
                table: "Voucher_Details",
                column: "VoucherHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Voucher_Details");
            migrationBuilder.DropTable(name: "Voucher_Header");
        }
    }
}
