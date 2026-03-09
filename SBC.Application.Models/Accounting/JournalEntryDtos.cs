namespace SBC.Application.Models.Accounting;

public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public List<JournalEntryLineDto> Lines { get; set; } = new();
    public decimal TotalDebit => Lines.Sum(x => x.Debit);
    public decimal TotalCredit => Lines.Sum(x => x.Credit);
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class JournalEntryLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class CreateJournalEntryDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CreateJournalEntryLineDto> Lines { get; set; } = new();
}

public class CreateJournalEntryLineDto
{
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class UpdateJournalEntryDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<UpdateJournalEntryLineDto> Lines { get; set; } = new();
}

public class UpdateJournalEntryLineDto
{
    public Guid? Id { get; set; } // Opcional para identificar líneas existentes
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class CreateJournalEntryLineForLineDto
{
    public Guid JournalEntryId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class UpdateJournalEntryLineForLineDto
{
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
