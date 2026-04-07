using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

/// <summary>
/// Interface for bulk import repository operations.
/// </summary>
public interface IBulkImportRepository : IBaseRepository<BulkImport>
{
    Task<(IEnumerable<BulkImport> Items, int TotalCount)> GetPagedAsync(
        string? fileName, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
}
