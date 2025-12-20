using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class updateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deff_JobTitle_Deff_Department_DepartmentId",
                table: "Deff_JobTitle");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_JobTitle_JobTitleId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_Deff_JobTitle_DepartmentId",
                table: "Deff_JobTitle");

            migrationBuilder.AddColumn<int>(
                name: "DeffDepartmentId",
                table: "UserInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "UserInfo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_DeffDepartmentId",
                table: "UserInfo",
                column: "DeffDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_DepartmentId",
                table: "UserInfo",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_Department_DeffDepartmentId",
                table: "UserInfo",
                column: "DeffDepartmentId",
                principalTable: "Deff_Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_Department_DepartmentId",
                table: "UserInfo",
                column: "DepartmentId",
                principalTable: "Deff_Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_JobTitle_JobTitleId",
                table: "UserInfo",
                column: "JobTitleId",
                principalTable: "Deff_JobTitle",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_Department_DeffDepartmentId",
                table: "UserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_Department_DepartmentId",
                table: "UserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Deff_JobTitle_JobTitleId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_DeffDepartmentId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_DepartmentId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "DeffDepartmentId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "UserInfo");

            migrationBuilder.CreateIndex(
                name: "IX_Deff_JobTitle_DepartmentId",
                table: "Deff_JobTitle",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deff_JobTitle_Deff_Department_DepartmentId",
                table: "Deff_JobTitle",
                column: "DepartmentId",
                principalTable: "Deff_Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Deff_JobTitle_JobTitleId",
                table: "UserInfo",
                column: "JobTitleId",
                principalTable: "Deff_JobTitle",
                principalColumn: "Id");
        }
    }
}
