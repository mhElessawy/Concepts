using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddVenderAndBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Def_Bank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Def_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenderCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccounttId = table.Column<int>(type: "int", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankAccountIBan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vender", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vender_Def_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Def_Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vender_Deff_City_CityId",
                        column: x => x.CityId,
                        principalTable: "Deff_City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vender_Deff_JobTitle_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "Deff_JobTitle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Def_Bank_BankCode",
                table: "Def_Bank",
                column: "BankCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vender_BankId",
                table: "Vender",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Vender_CityId",
                table: "Vender",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Vender_JobTitleId",
                table: "Vender",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vender_VenderCode",
                table: "Vender",
                column: "VenderCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vender");

            migrationBuilder.DropTable(
                name: "Def_Bank");
        }
    }
}
