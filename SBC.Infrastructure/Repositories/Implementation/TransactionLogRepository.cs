using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class TransactionLogRepository(SbcDbContext context) : BaseRepository<TransactionLog>(context), ITransactionLogRepository
{
    public async Task<(IEnumerable<(TransactionLog Log, string? Email)> Items, int TotalCount)> GetPagedAsync(
        string? action,
        string? entityName,
        TransactionStatus? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize)
    {
        var query = context.Set<TransactionLog>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(x => x.Action == action);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(x => x.EntityName == entityName);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(x => x.LogDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.LogDate <= toDate.Value);

        var totalCount = await query.CountAsync();

        var items = await (from log in query
                          join user in context.Users on log.UserId equals user.Id into userJoin
                          from user in userJoin.DefaultIfEmpty()
                          select new { Log = log, Email = user != null ? user.Email : null })
            .OrderByDescending(x => x.Log.LogDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items.Select(x => (x.Log, x.Email)), totalCount);
    }
}
