using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class JournalEntryRepository(SbcDbContext context) : BaseRepository<JournalEntry>(context), IJournalEntryRepository
{
    public override async Task<JournalEntry?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public override async Task<IEnumerable<JournalEntry>> GetAllAsync()
    {
        return await _dbSet
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .OrderByDescending(j => j.Date)
            .ThenByDescending(j => j.Code)
            .ToListAsync();
    }

    public async Task<string> GetLastCodeAsync(int year, int month)
    {
        var lastEntry = await _dbSet
            .Where(j => j.Year == year && j.Month == month)
            .OrderByDescending(j => j.Code)
            .FirstOrDefaultAsync();

        return lastEntry?.Code ?? string.Empty;
    }

    public async Task<JournalEntry?> GetByIdWithLinesAsync(Guid id)
    {
        return await _dbSet
            .Include(j => j.Lines)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<JournalEntry>> GetByDateRangeWithLinesAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .Where(j => j.Date >= startDate && j.Date <= endDate && j.IsPosted)
            .OrderBy(j => j.Date)
            .ToListAsync();
    }
}
