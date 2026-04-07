using Microsoft.AspNetCore.Http;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using System.Security.Claims;

namespace SBC.Application.Services.Implementation;

public class TransactionLogService(
    ITransactionLogRepository repository,
    IHttpContextAccessor httpContextAccessor) : ITransactionLogService
{
    public async Task LogTransactionAsync(
        Guid? userId,
        string action,
        string? entityName,
        string? entityId,
        string status,
        string? details = null,
        string? errorMessage = null)
    {
        var context = httpContextAccessor.HttpContext;
        var ipAddress = context?.Connection?.RemoteIpAddress?.ToString();

        if (userId == null)
        {
            var userIdClaim = context?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                userId = parsedUserId;
            }
        }

        var log = new TransactionLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Status = status,
            Details = details,
            ErrorMessage = errorMessage,
            IpAddress = ipAddress
        };

        await repository.AddAsync(log);
    }
}
