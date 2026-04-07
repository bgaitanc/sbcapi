using Microsoft.AspNetCore.Http;
using SBC.Application.Models.Common;
using SBC.Application.Models.Logging;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Enums;
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
        TransactionStatus status,
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
            IpAddress = ipAddress,
            LogDate = DateTime.UtcNow
        };

        await repository.AddAsync(log);
    }

    public async Task<PagedResultDto<TransactionLogDto>> GetPagedAsync(TransactionLogFilterDto filter)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            filter.Action,
            filter.EntityName,
            filter.Status,
            filter.FromDate,
            filter.ToDate,
            filter.PageNumber,
            filter.PageSize);

        return new PagedResultDto<TransactionLogDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    private static TransactionLogDto MapToDto(TransactionLog log)
    {
        return new TransactionLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = log.Action,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Status = log.Status,
            Details = log.Details,
            ErrorMessage = log.ErrorMessage,
            IpAddress = log.IpAddress,
            LogDate = log.LogDate
        };
    }
}
