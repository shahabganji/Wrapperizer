using Microsoft.EntityFrameworkCore.Migrations;

namespace Wrapperizer.Sample.Infra.Persistence.Migrations
{
    public partial class RenameStudentStatusToRegistrationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentStatus_RegistrationStatus",
                schema: "uni",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentStatus",
                schema: "uni",
                table: "StudentStatus");

            migrationBuilder.RenameTable(
                name: "StudentStatus",
                schema: "uni",
                newName: "RegistrationStatus",
                newSchema: "uni");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrationStatus",
                schema: "uni",
                table: "RegistrationStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_RegistrationStatus_RegistrationStatus",
                schema: "uni",
                table: "Students",
                column: "RegistrationStatus",
                principalSchema: "uni",
                principalTable: "RegistrationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_RegistrationStatus_RegistrationStatus",
                schema: "uni",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrationStatus",
                schema: "uni",
                table: "RegistrationStatus");

            migrationBuilder.RenameTable(
                name: "RegistrationStatus",
                schema: "uni",
                newName: "StudentStatus",
                newSchema: "uni");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentStatus",
                schema: "uni",
                table: "StudentStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentStatus_RegistrationStatus",
                schema: "uni",
                table: "Students",
                column: "RegistrationStatus",
                principalSchema: "uni",
                principalTable: "StudentStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
