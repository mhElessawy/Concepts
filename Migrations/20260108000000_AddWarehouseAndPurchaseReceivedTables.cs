using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseAndPurchaseReceivedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CostId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IVM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseType = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Deff_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Deff_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_Deff_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Deff_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRecieved_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecieveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecieveTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    RecieveNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BatchNo = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    VenderInvoiceNo = table.Column<int>(type: "int", nullable: false),
                    PaymentTerms = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approved = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRecieved_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Header_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Header_Vender_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Vender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Header_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRecieved_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRecievedHeaderId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    OrderQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecieveQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreeQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubUOMId = table.Column<int>(type: "int", nullable: false),
                    PackSize = table.Column<int>(type: "int", nullable: true),
                    ValueOrUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiredDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRecieved_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Details_Def_SubUOM_SubUOMId",
                        column: x => x.SubUOMId,
                        principalTable: "Def_SubUOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Details_PurchaseRecieved_Header_PurchaseRecievedHeaderId",
                        column: x => x.PurchaseRecievedHeaderId,
                        principalTable: "PurchaseRecieved_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseRecieved_Details_Store_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Store_Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_WarehouseCode",
                table: "Warehouse",
                column: "WarehouseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CountryId",
                table: "Warehouse",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_LocationId",
                table: "Warehouse",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_UserId",
                table: "Warehouse",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Header_RecieveNo",
                table: "PurchaseRecieved_Header",
                column: "RecieveNo");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Header_UserId",
                table: "PurchaseRecieved_Header",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Header_VenderId",
                table: "PurchaseRecieved_Header",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Header_WarehouseId",
                table: "PurchaseRecieved_Header",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Details_ItemId",
                table: "PurchaseRecieved_Details",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Details_PurchaseRecievedHeaderId",
                table: "PurchaseRecieved_Details",
                column: "PurchaseRecievedHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRecieved_Details_SubUOMId",
                table: "PurchaseRecieved_Details",
                column: "SubUOMId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseRecieved_Details");

            migrationBuilder.DropTable(
                name: "PurchaseRecieved_Header");

            migrationBuilder.DropTable(
                name: "Warehouse");
        }
    }
}
