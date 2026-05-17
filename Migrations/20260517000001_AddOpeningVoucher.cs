using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddOpeningVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpeningVoucher_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelayVoucher = table.Column<bool>(type: "bit", nullable: false),
                    Statement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningVoucher_Header", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpeningVoucher_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpeningVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    ChildAccountId = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    CostCenterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningVoucher_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningVoucher_Details_OpeningVoucher_Header_OpeningVoucherHeaderId",
                        column: x => x.OpeningVoucherHeaderId,
                        principalTable: "OpeningVoucher_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpeningVoucher_Details_Child_Account_ChildAccountId",
                        column: x => x.ChildAccountId,
                        principalTable: "Child_Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OpeningVoucher_Details_Deff_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "Deff_CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningVoucher_Header_VoucherNo",
                table: "OpeningVoucher_Header",
                column: "VoucherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpeningVoucher_Details_OpeningVoucherHeaderId",
                table: "OpeningVoucher_Details",
                column: "OpeningVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningVoucher_Details_ChildAccountId",
                table: "OpeningVoucher_Details",
                column: "ChildAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningVoucher_Details_CostCenterId",
                table: "OpeningVoucher_Details",
                column: "CostCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OpeningVoucher_Details");
            migrationBuilder.DropTable(name: "OpeningVoucher_Header");
        }
    }
}
