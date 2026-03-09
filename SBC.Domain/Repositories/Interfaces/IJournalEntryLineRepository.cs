using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

public interface IJournalEntryLineRepository : IBaseRepository<JournalEntryLine>
{
    Task<JournalEntryLine?> GetByIdWithJournalEntryAsync(Guid id);
    Task<IEnumerable<JournalEntryLine>> GetByJournalEntryIdAsync(Guid journalEntryId);
}
