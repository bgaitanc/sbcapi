using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Services.Interfaces;
using System.Net;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinancialReportsController(IFinancialReportService reportService) : SbcControllerBase
{
    [HttpGet("income-statement/excel")]
    public async Task<IActionResult> DownloadIncomeStatementExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] bool includeUnposted = false)
    {
        var fileBytes = await reportService.GenerateIncomeStatementExcelAsync(startDate, endDate, includeUnposted);
        var fileName = $"EstadoDeResultados_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("income-statement/pdf")]
    public async Task<IActionResult> DownloadIncomeStatementPdf([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] bool includeUnposted = false)
    {
        var fileBytes = await reportService.GenerateIncomeStatementPdfAsync(startDate, endDate, includeUnposted);
        var fileName = $"EstadoDeResultados_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
        return File(fileBytes, "application/pdf", fileName);
    }

    [HttpGet("balance-sheet/excel")]
    public async Task<IActionResult> DownloadBalanceSheetExcel([FromQuery] DateTime date, [FromQuery] bool includeUnposted = false)
    {
        var fileBytes = await reportService.GenerateBalanceSheetExcelAsync(date, includeUnposted);
        var fileName = $"BalanceGeneral_{date:yyyyMMdd}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("balance-sheet/pdf")]
    public async Task<IActionResult> DownloadBalanceSheetPdf([FromQuery] DateTime date, [FromQuery] bool includeUnposted = false)
    {
        var fileBytes = await reportService.GenerateBalanceSheetPdfAsync(date, includeUnposted);
        var fileName = $"BalanceGeneral_{date:yyyyMMdd}.pdf";
        return File(fileBytes, "application/pdf", fileName);
    }
}
