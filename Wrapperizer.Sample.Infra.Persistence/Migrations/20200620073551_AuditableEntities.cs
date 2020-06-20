using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wrapperizer.Sample.Infra.Persistence.Migrations
{
    public partial class AuditableEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                schema: "uni",
                table: "Students",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                schema: "uni",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "uni",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                schema: "uni",
                table: "Students");
        }
    }
}
