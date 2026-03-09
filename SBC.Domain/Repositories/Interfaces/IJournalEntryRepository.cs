using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

public interface IJournalEntryRepository : IBaseRepository<JournalEntry>
{
    Task<string> GetLastCodeAsync(int year, int month);
    Task<JournalEntry?> GetByIdWithLinesAsync(Guid id);
}
