using SBC.Application.Models.Common;
using SBC.Application.Models.Logging;
using SBC.Domain.Entities.Enums;

namespace SBC.Application.Services.Interfaces;

public interface ITransactionLogService
{
    Task LogTransactionAsync(
        Guid? userId,
        string action,
        string? entityName,
        string? entityId,
        TransactionStatus status,
        string? details = null,
        string? errorMessage = null);

    Task<PagedResultDto<TransactionLogDto>> GetPagedAsync(TransactionLogFilterDto filter);
}
