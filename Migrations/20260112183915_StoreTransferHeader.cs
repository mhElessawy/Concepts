using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class StoreTransferHeader : Migration
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
                    TransferCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransferNo = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_StoreTransfer_Header_Deff_Department_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "Deff_Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Header_Deff_Department_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "Deff_Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Header_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Header_Warehouse_FromWarehouseId",
                        column: x => x.FromWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransfer_Header_Warehouse_ToWarehouseId",
                        column: x => x.ToWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreTransfer_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTransferHeaderId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
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
                name: "IX_StoreTransfer_Header_FromDepartmentId",
                table: "StoreTransfer_Header",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Header_FromWarehouseId",
                table: "StoreTransfer_Header",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Header_ToDepartmentId",
                table: "StoreTransfer_Header",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransfer_Header_ToWarehouseId",
                table: "StoreTransfer_Header",
                column: "ToWarehouseId");

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
                name: "StoreTransfer_Details");

            migrationBuilder.DropTable(
                name: "StoreTransfer_Header");
        }
    }
}
