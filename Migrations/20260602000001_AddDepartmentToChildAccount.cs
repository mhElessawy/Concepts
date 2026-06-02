using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToChildAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Child_Account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Child_Account_DepartmentId",
                table: "Child_Account",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Child_Account_Deff_Department_DepartmentId",
                table: "Child_Account",
                column: "DepartmentId",
                principalTable: "Deff_Department",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Child_Account_Deff_Department_DepartmentId",
                table: "Child_Account");

            migrationBuilder.DropIndex(
                name: "IX_Child_Account_DepartmentId",
                table: "Child_Account");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Child_Account");
        }
    }
}
