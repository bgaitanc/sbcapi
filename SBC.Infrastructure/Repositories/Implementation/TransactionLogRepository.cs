using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using SBC.Infrastructure.Database;

namespace SBC.Infrastructure.Repositories.Implementation;

public class TransactionLogRepository(SbcDbContext context) : BaseRepository<TransactionLog>(context), ITransactionLogRepository
{
}
