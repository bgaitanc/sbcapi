using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

/// <summary>
/// Interface for services handling bulk import operations and history.
/// </summary>
public interface IBulkImportService
{
    /// <summary>
    /// Imports journal entries from an Excel file stream and records the operation.
    /// </summary>
    /// <param name="excelStream">The Excel file stream.</param>
    /// <param name="fileName">The name of the imported file.</param>
    /// <returns>A result object containing counts and error details.</returns>
    Task<BulkJournalEntryImportResultDto> ImportFromExcelAsync(Stream excelStream, string fileName);

    /// <summary>
    /// Retrieves the history of bulk import operations.
    /// </summary>
    /// <returns>A collection of bulk import records.</returns>
    Task<IEnumerable<BulkImportDto>> GetHistoryAsync();
}
