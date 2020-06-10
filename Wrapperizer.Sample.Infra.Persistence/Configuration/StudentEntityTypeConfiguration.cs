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
                x.Property("_nationalCode")
                    .UsePropertyAccessMode(PropertyAccessMode.Field)
                    .HasColumnName("NationalCode")
                    .IsRequired();
            });
            
            builder.Property("_status")
                .IsRequired()
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("_status");

            builder.HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey("_status")
                .IsRequired();
        }
    }
}
