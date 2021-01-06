using Funx.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence.Migrations
{
    public partial class SeedRegistrationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var statuses = Enumeration.GetAll<RegistrationStatus>();
            
            statuses.ForEach(status =>
            {
                migrationBuilder.InsertData("RegistrationStatus",
                    new[] {nameof(RegistrationStatus.Id), nameof(RegistrationStatus.Name)},
                    new object[] {status.Id, status.Name},
                    "uni");
            });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
