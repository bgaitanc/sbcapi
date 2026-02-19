using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBC.Domain.Entities.Accounting;

namespace SBC.Infrastructure.Database.Configurations;

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines", "accounting");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Debit).IsRequired().HasColumnType("money");
        builder.Property(x => x.Credit).IsRequired().HasColumnType("money");

        builder.HasOne(x => x.JournalEntry).WithMany(x => x.Lines).HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_JournalEntryLines_Debit_NoNegative", "[Debit] >= 0");
            t.HasCheckConstraint("CK_JournalEntryLines_Credit_NoNegative", "[Credit] >= 0");

            t.HasCheckConstraint("CK_JournalEntryLines_HasValue", "[Debit] > 0 OR [Credit] > 0");

            // Opcional: Si tu lógica de negocio exige que una línea sea EXCLUSIVAMENTE
            // débito O crédito (no ambos en la misma línea), usa este constraint en su lugar:
            // t.HasCheckConstraint("CK_JournalEntryLines_Exclusive", 
            //    "([Debit] > 0 AND [Credit] = 0) OR ([Debit] = 0 AND [Credit] > 0)");
        });
    }
}