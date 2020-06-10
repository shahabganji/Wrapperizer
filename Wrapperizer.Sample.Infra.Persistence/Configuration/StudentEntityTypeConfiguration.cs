using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence.Configuration
{
    internal sealed class StudentEntityTypeConfiguration
        : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.Ignore(p => p.DomainEvents);

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .IsRequired();
            
            builder.Property(p => p.FirstName)
                .IsUnicode()
                .IsRequired();
            
            builder.Property(p => p.LastName)
                .IsUnicode()
                .IsRequired();

            builder.Property(p => p.DateOfBirth)
                .IsRequired();


            builder.OwnsOne(p => p.NationalCode, x =>
            {
                x.Property<string>("_nationalCode")
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("NationalCode")
                    .IsRequired();
            });
            
            builder.Property<int>("_registrationStatus")
                .IsRequired()
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RegistrationStatus");

            builder.HasOne(p => p.RegistrationStatus)
                .WithMany()
                .HasForeignKey("_registrationStatus")
                .IsRequired();
        }
    }
}
