using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Accounting;

namespace SBC.Infrastructure.Database.Configurations;

public class AccountingPeriodConfiguration : IEntityTypeConfiguration<AccountingPeriod>
{
    public void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        builder.ToTable("AccountingPeriods", "accounting");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Year).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Month).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.IsClosed).IsRequired().HasDefaultValue(false).HasColumnType("bit");
        builder.Property(x => x.ClosedAt).HasColumnType("datetime2");
        builder.Property(x => x.ClosedBy).HasMaxLength(100).HasColumnType("nvarchar(100)");
        
        builder.Property(x => x.CreatedBy).HasMaxLength(100).HasColumnType("nvarchar(100)");
        builder.Property(x => x.UpdatedBy).HasMaxLength(100).HasColumnType("nvarchar(100)");

        builder.HasIndex(x => new { x.Year, x.Month }).IsUnique();

        builder.HasOne(x => x.ClosingJournalEntry)
            .WithMany()
            .HasForeignKey(x => x.ClosingJournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_AccountingPeriods_Month_Valid", "[Month] >= 1 AND [Month] <= 12");
            t.HasCheckConstraint("CK_AccountingPeriods_Year_Valid", "[Year] >= 1900");
        });
    }
}
