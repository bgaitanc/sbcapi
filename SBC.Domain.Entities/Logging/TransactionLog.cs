using SBC.Domain.Entities.Base;

namespace SBC.Domain.Entities.Logging;

public class TransactionLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string Status { get; set; } = string.Empty; // Success, Failure, ValidationError
    public string? ErrorMessage { get; set; }
    public string? IpAddress { get; set; }
}
