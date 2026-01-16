using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class all : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            


            migrationBuilder.CreateTable(
                name: "StoreTransfer_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferNo = table.Column<int>(type: "int", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    RequestedBy = table.Column<int>(type: "int", nullable: false),
                    AprovedBy = table.Column<int>(type: "int", nullable: false),
                    TransferStatus = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransferTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    FromWarehouseId = table.Column<int>(type: "int", nullable: false),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: false),
                    ToWarehouseId = table.Column<int>(type: "int", nullable: false),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransfer_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Header_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id");
                });

         
            migrationBuilder.CreateTable(
                name: "StoreTransfer_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTransferHeaderId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UOMId = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceType = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostType = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalType = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubUOMId = table.Column<int>(type: "int", nullable: false),
                    PackSize = table.Column<int>(type: "int", nullable: true),
                    ValueOrUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiredDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransfer_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_Def_SubUOM_SubUOMId",
                        column: x => x.SubUOMId,
                        principalTable: "Def_SubUOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_Def_UOM_UOMId",
                        column: x => x.UOMId,
                        principalTable: "Def_UOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_Deff_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Deff_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_Deff_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "Deff_SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_StoreTransfer_Header_StoreTransferHeaderId",
                        column: x => x.StoreTransferHeaderId,
                        principalTable: "StoreTransfer_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Details_Store_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Store_Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });



            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_CategoryId",
                table: "StoreTransfer_Details",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_ItemId",
                table: "StoreTransfer_Details",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_StoreTransferHeaderId",
                table: "StoreTransfer_Details",
                column: "StoreTransferHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_SubCategoryId",
                table: "StoreTransfer_Details",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_SubUOMId",
                table: "StoreTransfer_Details",
                column: "SubUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Details_UOMId",
                table: "StoreTransfer_Details",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Header_TransferNo",
                table: "StoreTransfer_Header",
                column: "TransferNo");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Header_UserId",
                table: "StoreTransfer_Header",
                column: "UserId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "PurchaseRecieved_Details");

            migrationBuilder.DropTable(
                name: "PurchaseRequestDetails");

            migrationBuilder.DropTable(
                name: "StoreTransfer_Details");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeaders");

            migrationBuilder.DropTable(
                name: "PurchaseRecieved_Header");

            migrationBuilder.DropTable(
                name: "PurchaseRequestHeaders");

            migrationBuilder.DropTable(
                name: "StoreTransfer_Header");

            migrationBuilder.DropTable(
                name: "Store_Item");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Vender");

            migrationBuilder.DropTable(
                name: "Def_SubUOM");

            migrationBuilder.DropTable(
                name: "Deff_SubCategory");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "Def_Bank");

            migrationBuilder.DropTable(
                name: "Deff_City");

            migrationBuilder.DropTable(
                name: "Def_UOM");

            migrationBuilder.DropTable(
                name: "Deff_Category");

            migrationBuilder.DropTable(
                name: "Deff_Department");

            migrationBuilder.DropTable(
                name: "Deff_JobTitle");

            migrationBuilder.DropTable(
                name: "Deff_Location");

            migrationBuilder.DropTable(
                name: "Deff_Country");
        }
    }
}
