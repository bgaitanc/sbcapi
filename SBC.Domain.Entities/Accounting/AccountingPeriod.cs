using SBC.Domain.Entities.Base;

namespace SBC.Domain.Entities.Accounting;

public class AccountingPeriod : BaseEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    
    // Opcional: Relación con el asiento de cierre
    public Guid? ClosingJournalEntryId { get; set; }
    public JournalEntry? ClosingJournalEntry { get; set; }
}
