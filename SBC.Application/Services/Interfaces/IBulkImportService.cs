using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;

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

    /// <summary>
    /// Retrieves a paged and filtered history of bulk import operations.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>A paged result of bulk import records.</returns>
    Task<PagedResultDto<BulkImportDto>> GetPagedHistoryAsync(BulkImportFilterDto filter);

    /// <summary>
    /// Generates an Excel template for bulk journal entry imports with sample data.
    /// </summary>
    /// <returns>A byte array containing the Excel file.</returns>
    Task<byte[]> GenerateBulkImportTemplateAsync();
}
