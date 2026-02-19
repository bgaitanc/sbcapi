using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;

namespace SBC.Infrastructure.Database.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", "accounting");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(20).HasColumnType("varchar(20)");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Type).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne(x => x.ParentAccount).WithMany(x => x.SubAccounts).HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Accounts_Code_NotEmpty", "LEN([Code]) > 0");
            t.HasCheckConstraint("CK_Accounts_Name_NotEmpty", "LEN([Name]) > 0");

            var validTypes = Enum.GetNames(typeof(AccountType))
                .Select(e => $"'{e}'");
            
            var sqlInClause = string.Join(", ", validTypes);
            
            t.HasCheckConstraint("CK_Accounts_Type_Valid", $"[Type] IN ({sqlInClause})");
        });
    }
}