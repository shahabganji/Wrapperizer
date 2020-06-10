using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence.Configuration
{
    internal sealed class StudentStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<StudentStatus>
    {
        public void Configure(EntityTypeBuilder<StudentStatus> builder)
        {
            builder.ToTable("StudentStatus");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
