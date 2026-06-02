using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class CashTransactionExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelatedVoucherNo",
                table: "CashTransaction_Header",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "CashTransaction_Header",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountNote",
                table: "CashTransaction_Header",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "RelatedVoucherNo", table: "CashTransaction_Header");
            migrationBuilder.DropColumn(name: "Amount", table: "CashTransaction_Header");
            migrationBuilder.DropColumn(name: "DiscountNote", table: "CashTransaction_Header");
        }
    }
}