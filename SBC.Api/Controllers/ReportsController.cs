using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController(IFinancialReportService service) : SbcControllerBase
{
    [HttpGet("income-statement")]
    public async Task<ActionResult<IncomeStatementDto>> GetIncomeStatement([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        return await ExecuteServiceAsync(() => service.GetIncomeStatementAsync(startDate, endDate));
    }

    [HttpGet("balance-sheet")]
    public async Task<ActionResult<BalanceSheetDto>> GetBalanceSheet([FromQuery] DateTime date)
    {
        return await ExecuteServiceAsync(() => service.GetBalanceSheetAsync(date));
    }
}
