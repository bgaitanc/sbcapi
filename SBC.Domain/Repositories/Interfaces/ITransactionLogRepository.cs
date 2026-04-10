using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;

namespace SBC.Domain.Repositories.Interfaces;

public interface ITransactionLogRepository : IBaseRepository<TransactionLog>
{
    Task<(IEnumerable<(TransactionLog Log, string? Email)> Items, int TotalCount)> GetPagedAsync(
        string? action,
        string? entityName,
        TransactionStatus? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize);
}
