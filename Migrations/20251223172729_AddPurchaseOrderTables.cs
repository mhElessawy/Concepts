using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrderHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    PurchaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PurchaseStatus = table.Column<int>(type: "int", nullable: false),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_Deff_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Deff_Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_Vender_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Vender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    AvQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubUnitId = table.Column<int>(type: "int", nullable: false),
                    PackSize = table.Column<int>(type: "int", nullable: true),
                    ValueOrUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    freeQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Def_SubUOM_SubUnitId",
                        column: x => x.SubUnitId,
                        principalTable: "Def_SubUOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Deff_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "Deff_SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_PurchaseOrderHeaders_PurchaseOrderHeaderId",
                        column: x => x.PurchaseOrderHeaderId,
                        principalTable: "PurchaseOrderHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Store_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Store_Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_ItemId",
                table: "PurchaseOrderDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderHeaderId",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_SubCategoryId",
                table: "PurchaseOrderDetails",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_SubUnitId",
                table: "PurchaseOrderDetails",
                column: "SubUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_DepartmentId",
                table: "PurchaseOrderHeaders",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_UserId",
                table: "PurchaseOrderHeaders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_VenderId",
                table: "PurchaseOrderHeaders",
                column: "VenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeaders");
        }
    }
}
