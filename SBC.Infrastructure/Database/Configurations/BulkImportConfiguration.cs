using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Accounting;

namespace SBC.Infrastructure.Database.Configurations;

public class BulkImportConfiguration : IEntityTypeConfiguration<BulkImport>
{
    public void Configure(EntityTypeBuilder<BulkImport> builder)
    {
        builder.ToTable("BulkImports", "accounting");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255).HasColumnType("nvarchar(255)");
        builder.HasIndex(x => x.FileName);
        builder.Property(x => x.SuccessCount).IsRequired().HasColumnType("int");
        builder.Property(x => x.ErrorCount).IsRequired().HasColumnType("int");
        builder.Property(x => x.TotalCount).IsRequired().HasColumnType("int");

        builder.Property(x => x.CreatedAt).IsRequired().HasColumnType("datetime2");
        builder.HasIndex(x => x.CreatedAt);
        builder.Property(x => x.CreatedBy).IsRequired(false).HasMaxLength(100).HasColumnType("nvarchar(100)");
        builder.Property(x => x.UpdatedAt).IsRequired(false).HasColumnType("datetime2");
        builder.Property(x => x.UpdatedBy).IsRequired(false).HasMaxLength(100).HasColumnType("nvarchar(100)");
    }
}
