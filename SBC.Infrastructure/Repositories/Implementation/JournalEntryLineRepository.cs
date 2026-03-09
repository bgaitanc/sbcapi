using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class JournalEntryLineRepository(SbcDbContext context) : BaseRepository<JournalEntryLine>(context), IJournalEntryLineRepository
{
    public override async Task<JournalEntryLine?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.Account)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<JournalEntryLine?> GetByIdWithJournalEntryAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.JournalEntry)
                .ThenInclude(j => j.Lines)
            .Include(l => l.Account)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<JournalEntryLine>> GetByJournalEntryIdAsync(Guid journalEntryId)
    {
        return await _dbSet
            .Include(l => l.Account)
            .Where(l => l.JournalEntryId == journalEntryId)
            .ToListAsync();
    }
}
