using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;
using SBC.Application.Services.Interfaces;

using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Logging;

namespace SBC.Api.Controllers;

/// <summary>
/// Controller for managing bulk import operations and history.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BulkImportsController(IBulkImportService service) : SbcControllerBase
{
    /// <summary>
    /// Retrieves a paged and filtered history of bulk journal entry imports.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>A paged result of bulk import records.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<BulkImportDto>>> GetPagedHistory([FromQuery] BulkImportFilterDto filter)
    {
        return await ExecuteServiceAsync(() => service.GetPagedHistoryAsync(filter), HttpStatusCode.OK, TransactionActions.GetBulkImports, nameof(BulkImport), filter);
    }

    /// <summary>
    /// Imports journal entries from an Excel file and records the operation.
    /// </summary>
    /// <param name="file">The Excel file containing journal entries.</param>
    /// <returns>A summary of the import operation results.</returns>
    [HttpPost]
    public async Task<ActionResult<BulkJournalEntryImportResultDto>> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No se ha proporcionado un archivo o el archivo está vacío.");

        using var stream = file.OpenReadStream();
        return await ExecuteServiceAsync(() => service.ImportFromExcelAsync(stream, file.FileName));
    }
}
