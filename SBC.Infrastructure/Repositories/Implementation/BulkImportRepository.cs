using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

/// <summary>
/// Implementation of the bulk import repository.
/// </summary>
public class BulkImportRepository(SbcDbContext context) : BaseRepository<BulkImport>(context), IBulkImportRepository
{
}
