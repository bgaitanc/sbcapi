using SBC.Application.Models.Common;
using SBC.Domain.Entities.Enums;

namespace SBC.Application.Models.Logging;

public class TransactionLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public TransactionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? IpAddress { get; set; }
    public DateTime LogDate { get; set; }
}

public class TransactionLogFilterDto : BaseFilterDto
{
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public TransactionStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
