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
                    VoucherType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voucher_Header_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Voucher_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Voucher_Header_VoucherHeaderId",
                        column: x => x.VoucherHeaderId,
                        principalTable: "Voucher_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Child_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Child_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Voucher_Details_Deff_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "Deff_CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Header_VoucherNo",
                table: "Voucher_Header",
                column: "VoucherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Header_UserId",
                table: "Voucher_Header",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_VoucherHeaderId",
                table: "Voucher_Details",
                column: "VoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_AccountId",
                table: "Voucher_Details",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_Details_CostCenterId",
                table: "Voucher_Details",
                column: "CostCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Voucher_Details");
            migrationBuilder.DropTable(name: "Voucher_Header");
        }
    }
}
