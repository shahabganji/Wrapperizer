using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wrapperizer.Sample.Infra.Persistence.Migrations
{
    public partial class InitializeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "uni");

            migrationBuilder.CreateTable(
                name: "StudentStatus",
                schema: "uni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "uni",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    NationalCode = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false),
                    RegistrationStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_StudentStatus_RegistrationStatus",
                        column: x => x.RegistrationStatus,
                        principalSchema: "uni",
                        principalTable: "StudentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_RegistrationStatus",
                schema: "uni",
                table: "Students",
                column: "RegistrationStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students",
                schema: "uni");

            migrationBuilder.DropTable(
                name: "StudentStatus",
                schema: "uni");
        }
    }
}
