using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Accounting;

namespace SBC.Infrastructure.Database.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries", "accounting");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(20).HasColumnType("varchar(20)");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Date).IsRequired().HasColumnType("datetime2");
        builder.Property(x => x.Day).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Month).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Year).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(255).HasColumnType("nvarchar(255)");
        builder.Property(x => x.IsPosted).IsRequired().HasDefaultValue(false).HasColumnType("bit");

        builder.HasMany(x => x.Lines).WithOne(x => x.JournalEntry).HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_JournalEntries_Code_NotEmpty", "LEN([Code]) > 0");

            t.HasCheckConstraint("CK_JournalEntries_Description_NotEmpty", "[Description] IS NULL OR LEN([Description]) > 0");

            t.HasCheckConstraint("CK_JournalEntries_Month_Valid", "[Month] >= 1 AND [Month] <= 12");
            t.HasCheckConstraint("CK_JournalEntries_Day_Valid", "[Day] >= 1 AND [Day] <= 31");
            t.HasCheckConstraint("CK_JournalEntries_Year_Valid", "[Year] >= 1900");
        });
    }
}