using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Logging;

namespace SBC.Infrastructure.Database.Configurations;

public class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
{
    public void Configure(EntityTypeBuilder<TransactionLog> builder)
    {
        builder.ToTable("TransactionLogs", "logging");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityName)
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.Details)
            .HasMaxLength(4000);

        builder.Property(x => x.LogDate)
            .IsRequired();

        // Indexes for performance optimization
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.EntityName);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.LogDate);
    }
}
