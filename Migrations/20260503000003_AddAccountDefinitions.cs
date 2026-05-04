using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concept.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Def_AccountType
            migrationBuilder.CreateTable(
                name: "Def_AccountType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Def_AccountType", x => x.Id); });

            migrationBuilder.CreateIndex("IX_Def_AccountType_Code", "Def_AccountType", "Code", unique: true);

            // Create Def_AccountEffect
            migrationBuilder.CreateTable(
                name: "Def_AccountEffect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Def_AccountEffect", x => x.Id); });

            migrationBuilder.CreateIndex("IX_Def_AccountEffect_Code", "Def_AccountEffect", "Code", unique: true);

            // Create Def_NatureOfAccount
            migrationBuilder.CreateTable(
                name: "Def_NatureOfAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Def_NatureOfAccount", x => x.Id); });

            migrationBuilder.CreateIndex("IX_Def_NatureOfAccount_Code", "Def_NatureOfAccount", "Code", unique: true);

            // Alter Child_Account: drop old string columns, add FK columns
            migrationBuilder.DropColumn(name: "AccountType",      table: "Child_Account");
            migrationBuilder.DropColumn(name: "AccountEffect",    table: "Child_Account");
            migrationBuilder.DropColumn(name: "NatureOfAccount",  table: "Child_Account");

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeId", table: "Child_Account", type: "int", nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountEffectId", table: "Child_Account", type: "int", nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NatureOfAccountId", table: "Child_Account", type: "int", nullable: true);

            migrationBuilder.CreateIndex("IX_Child_Account_AccountTypeId",     "Child_Account", "AccountTypeId");
            migrationBuilder.CreateIndex("IX_Child_Account_AccountEffectId",   "Child_Account", "AccountEffectId");
            migrationBuilder.CreateIndex("IX_Child_Account_NatureOfAccountId", "Child_Account", "NatureOfAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Child_Account_Def_AccountType_AccountTypeId",
                table: "Child_Account", column: "AccountTypeId",
                principalTable: "Def_AccountType", principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Child_Account_Def_AccountEffect_AccountEffectId",
                table: "Child_Account", column: "AccountEffectId",
                principalTable: "Def_AccountEffect", principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Child_Account_Def_NatureOfAccount_NatureOfAccountId",
                table: "Child_Account", column: "NatureOfAccountId",
                principalTable: "Def_NatureOfAccount", principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Child_Account_Def_AccountType_AccountTypeId",       "Child_Account");
            migrationBuilder.DropForeignKey("FK_Child_Account_Def_AccountEffect_AccountEffectId",   "Child_Account");
            migrationBuilder.DropForeignKey("FK_Child_Account_Def_NatureOfAccount_NatureOfAccountId","Child_Account");

            migrationBuilder.DropIndex("IX_Child_Account_AccountTypeId",     "Child_Account");
            migrationBuilder.DropIndex("IX_Child_Account_AccountEffectId",   "Child_Account");
            migrationBuilder.DropIndex("IX_Child_Account_NatureOfAccountId", "Child_Account");

            migrationBuilder.DropColumn(name: "AccountTypeId",     table: "Child_Account");
            migrationBuilder.DropColumn(name: "AccountEffectId",   table: "Child_Account");
            migrationBuilder.DropColumn(name: "NatureOfAccountId", table: "Child_Account");

            migrationBuilder.AddColumn<string>(name: "AccountType",     table: "Child_Account", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "AccountEffect",   table: "Child_Account", type: "nvarchar(max)", nullable: false, defaultValue: "Debit");
            migrationBuilder.AddColumn<string>(name: "NatureOfAccount", table: "Child_Account", type: "nvarchar(max)", nullable: false, defaultValue: "");

            migrationBuilder.DropTable("Def_AccountType");
            migrationBuilder.DropTable("Def_AccountEffect");
            migrationBuilder.DropTable("Def_NatureOfAccount");
        }
    }
}
