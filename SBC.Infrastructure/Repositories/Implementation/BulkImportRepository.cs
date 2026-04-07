using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

/// <summary>
/// Implementation of the bulk import repository.
/// </summary>
public class BulkImportRepository(SbcDbContext context) : BaseRepository<BulkImport>(context), IBulkImportRepository
{
    public async Task<(IEnumerable<BulkImport> Items, int TotalCount)> GetPagedAsync(
        string? fileName, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            query = query.Where(i => i.FileName.Contains(fileName));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(i => i.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(i => i.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
