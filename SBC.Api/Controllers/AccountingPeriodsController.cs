using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Logging;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountingPeriodsController(IAccountingPeriodService service) : SbcControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountingPeriodDto>>> GetAll()
    {
        return await ExecuteServiceAsync(() => service.GetAllPeriodsAsync(), HttpStatusCode.OK, TransactionActions.GetAccountingPeriods, nameof(AccountingPeriod));
    }

    [HttpGet("{year}/{month}")]
    public async Task<ActionResult<AccountingPeriodDto>> GetByPeriod(int year, int month)
    {
        return await ExecuteServiceAsync(async () =>
        {
            var period = await service.GetByPeriodAsync(year, month);
            return period ?? throw new Exception("Periodo no encontrado");
        }, HttpStatusCode.OK, TransactionActions.GetAccountingPeriods, nameof(AccountingPeriod), new { year, month });
    }

    [HttpPost]
    public async Task<ActionResult<AccountingPeriodDto>> Create([FromBody] CreateAccountingPeriodRequest request)
    {
        return await ExecuteServiceAsync(
            () => service.CreatePeriodAsync(request.Year, request.Month),
            HttpStatusCode.Created);
    }

    [HttpPost("close")]
    public async Task<ActionResult<AccountingPeriodDto>> ClosePeriod([FromBody] ClosePeriodRequest request)
    {
        return await ExecuteServiceAsync(
            () => service.ClosePeriodAsync(request.Year, request.Month, request.EquityAccountId), 
            HttpStatusCode.Created);
    }
}
