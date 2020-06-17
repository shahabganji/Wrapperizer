using Microsoft.EntityFrameworkCore.Migrations;

namespace Wrapperizer.Sample.Infra.Persistence.Migrations
{
    public partial class SofDeleteForStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                schema: "uni",
                table: "Students",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                schema: "uni",
                table: "Students");
        }
    }
}
