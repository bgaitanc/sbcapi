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

    public async Task<IEnumerable<(Guid AccountId, string Code, string Name, int Count, decimal Total)>> GetTopAccountsAsync(int count)
    {
        var result = await _dbSet
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new
            {
                g.Key.AccountId,
                g.Key.Code,
                g.Key.Name,
                Count = g.Count(),
                Total = g.Sum(l => l.Debit + l.Credit)
            })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToListAsync();

        return result.Select(x => (x.AccountId, x.Code, x.Name, x.Count, x.Total));
    }

    public async Task<(IEnumerable<JournalEntryLine> Items, int TotalCount)> GetPagedAsync(
        Guid? accountId, DateTime? fromDate, DateTime? toDate, decimal? minAmount, decimal? maxAmount, int pageNumber, int pageSize)
    {
        var query = _dbSet.AsNoTracking();

        if (accountId.HasValue)
        {
            query = query.Where(l => l.AccountId == accountId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.JournalEntry.Date >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(l => l.JournalEntry.Date <= toDate.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(l => l.Debit >= minAmount.Value || l.Credit >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(l => l.Debit <= maxAmount.Value && l.Credit <= maxAmount.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(l => l.Account)
            .Include(l => l.JournalEntry)
            .OrderByDescending(l => l.JournalEntry.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
