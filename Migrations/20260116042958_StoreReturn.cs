using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class StoreReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreReturn_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnNo = table.Column<int>(type: "int", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ReturnType = table.Column<int>(type: "int", nullable: false),
                    FromWarehouseId = table.Column<int>(type: "int", nullable: false),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: false),
                    ToWarehouseId = table.Column<int>(type: "int", nullable: false),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: false),
                    ReturnStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReturn_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Header_UserInfo_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StoreReturn_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreReturnHeaderId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_StoreReturn_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_Def_SubUOM_SubUOMId",
                        column: x => x.SubUOMId,
                        principalTable: "Def_SubUOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_Def_UOM_UOMId",
                        column: x => x.UOMId,
                        principalTable: "Def_UOM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_Deff_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Deff_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_Deff_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "Deff_SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_StoreReturn_Header_StoreReturnHeaderId",
                        column: x => x.StoreReturnHeaderId,
                        principalTable: "StoreReturn_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreReturn_Details_Store_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Store_Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_CategoryId",
                table: "StoreReturn_Details",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_ItemId",
                table: "StoreReturn_Details",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_StoreReturnHeaderId",
                table: "StoreReturn_Details",
                column: "StoreReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_SubCategoryId",
                table: "StoreReturn_Details",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_SubUOMId",
                table: "StoreReturn_Details",
                column: "SubUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Details_UOMId",
                table: "StoreReturn_Details",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Header_ReturnNo",
                table: "StoreReturn_Header",
                column: "ReturnNo");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturn_Header_UserId",
                table: "StoreReturn_Header",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreReturn_Details");

            migrationBuilder.DropTable(
                name: "StoreReturn_Header");
        }
    }
}
