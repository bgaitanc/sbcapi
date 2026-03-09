namespace SBC.Application.Models.Accounting;

public class AccountingPeriodDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public Guid? ClosingJournalEntryId { get; set; }
}

public class ClosePeriodRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public Guid EquityAccountId { get; set; }
}
