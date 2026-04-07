namespace SBC.Application.Services.Interfaces;

public interface ITransactionLogService
{
    Task LogTransactionAsync(Guid? userId, string action, string? entityName, string? entityId, string status, string? details = null, string? errorMessage = null);
}
