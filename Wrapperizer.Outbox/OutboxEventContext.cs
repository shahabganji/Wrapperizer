using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrapperizer.Outbox
{
    public class OutboxEventContext : DbContext
    {
        private const string DefaultSchema = "outbox"; 
        public OutboxEventContext(DbContextOptions<OutboxEventContext> options) : base(options)
        {
        }

        public DbSet<IntegrationEventLogEntry> Outbox { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {          
            builder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
        }

        private void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable(nameof(Outbox) , DefaultSchema);

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();

        }
    }
}
