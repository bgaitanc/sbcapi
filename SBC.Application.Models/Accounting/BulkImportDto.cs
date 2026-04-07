namespace SBC.Application.Models.Accounting;

/// <summary>
/// Data transfer object for bulk import records.
/// </summary>
public class BulkImportDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
