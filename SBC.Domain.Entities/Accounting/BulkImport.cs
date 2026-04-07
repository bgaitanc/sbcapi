using SBC.Domain.Entities.Base;

namespace SBC.Domain.Entities.Accounting;

/// <summary>
/// Represents a record of a bulk journal entry import operation.
/// Tracks metadata such as the file name, import date, and counts of successful and failed entries.
/// </summary>
public class BulkImport : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the file that was imported.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the count of journal entries successfully created during the import.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Gets or sets the count of rows or entries that failed to import due to validation errors.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of journal entries found in the import file.
    /// </summary>
    public int TotalCount { get; set; }
}
