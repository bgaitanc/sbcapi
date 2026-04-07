using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

public interface IJournalEntryLineRepository : IBaseRepository<JournalEntryLine>
{
    Task<JournalEntryLine?> GetByIdWithJournalEntryAsync(Guid id);
    Task<IEnumerable<JournalEntryLine>> GetByJournalEntryIdAsync(Guid journalEntryId);
    Task<IEnumerable<(Guid AccountId, string Code, string Name, int Count, decimal Total)>> GetTopAccountsAsync(int count);
    Task<(IEnumerable<JournalEntryLine> Items, int TotalCount)> GetPagedAsync(
        Guid? accountId, DateTime? fromDate, DateTime? toDate, decimal? minAmount, decimal? maxAmount, int pageNumber, int pageSize);
}
