using Concept.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Concept.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261118000000_AutomateCategoryCodesAndEnforceUniqueNames")]
public partial class AutomateCategoryCodesAndEnforceUniqueNames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF EXISTS (
                SELECT 1
                FROM [Deff_Category]
                GROUP BY LTRIM(RTRIM([Name])) COLLATE Latin1_General_100_CI_AS
                HAVING COUNT(*) > 1
            )
                THROW 51000, 'Duplicate category names already exist. Give each existing category a unique name before applying this migration.', 1;
            """);

        migrationBuilder.CreateSequence<long>(name: "CategoryCodeSequence");

        migrationBuilder.AddColumn<string>(
            name: "NormalizedName",
            table: "Deff_Category",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            computedColumnSql: "LTRIM(RTRIM([Name])) COLLATE Latin1_General_100_CI_AS",
            stored: true);

        migrationBuilder.CreateIndex(
            name: "IX_Deff_Category_NormalizedName",
            table: "Deff_Category",
            column: "NormalizedName",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Deff_Category_NormalizedName", table: "Deff_Category");
        migrationBuilder.DropColumn(name: "NormalizedName", table: "Deff_Category");
        migrationBuilder.DropSequence(name: "CategoryCodeSequence");
    }
}
