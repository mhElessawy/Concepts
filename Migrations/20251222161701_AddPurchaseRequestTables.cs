using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseRequestTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeffLocationId",
                table: "UserInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "UserInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Deff_Department",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deff_Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deff_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    RequestNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RequestedStatus = table.Column<int>(type: "int", nullable: false),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestHeaders_Deff_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Deff_Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestHeaders_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestHeaders_Vender_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Vender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequestDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestHeaderId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubUnitId = table.Column<int>(type: "int", nullable: false),
                    PackSize = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDetails_Def_SubUOM_SubUnitId",
                        column: x => x.SubUnitId,
                        principalTable: "Def_SubUOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDetails_Deff_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "Deff_SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDetails_PurchaseRequestHeaders_PurchaseRequestHeaderId",
                        column: x => x.PurchaseRequestHeaderId,
                        principalTable: "PurchaseRequestHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRequestDetails_Store_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Store_Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_DeffLocationId",
                table: "UserInfo",
                column: "DeffLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_LocationId",
                table: "UserInfo",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDetails_ItemId",
                table: "PurchaseRequestDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDetails_PurchaseRequestHeaderId",
                table: "PurchaseRequestDetails",
                column: "PurchaseRequestHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDetails_SubCategoryId",
                table: "PurchaseRequestDetails",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestDetails_SubUnitId",
                table: "PurchaseRequestDetails",
                column: "SubUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHeaders_DepartmentId",
                table: "PurchaseRequestHeaders",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHeaders_UserId",
                table: "PurchaseRequestHeaders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequestHeaders_VenderId",
                table: "PurchaseRequestHeaders",
                column: "VenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_Location_DeffLocationId",
                table: "UserInfo",
                column: "DeffLocationId",
                principalTable: "Deff_Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_Location_LocationId",
                table: "UserInfo",
                column: "LocationId",
                principalTable: "Deff_Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_Location_DeffLocationId",
                table: "UserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_Location_LocationId",
                table: "UserInfo");

            migrationBuilder.DropTable(
                name: "Deff_Location");

            migrationBuilder.DropTable(
                name: "PurchaseRequestDetails");

            migrationBuilder.DropTable(
                name: "PurchaseRequestHeaders");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_DeffLocationId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_LocationId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "DeffLocationId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Deff_Department");
        }
    }
}
