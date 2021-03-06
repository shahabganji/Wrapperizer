using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence.Configuration
{
    internal sealed class RegistrationStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<RegistrationStatus>
    {
        public void Configure(EntityTypeBuilder<RegistrationStatus> builder)
        {
            builder.ToTable("RegistrationStatus");

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
