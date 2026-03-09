using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountingPeriodsController(IAccountingPeriodService service) : SbcControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountingPeriodDto>>> GetAll()
    {
        return await ExecuteServiceAsync(() => service.GetAllPeriodsAsync());
    }

    [HttpGet("{year}/{month}")]
    public async Task<ActionResult<AccountingPeriodDto>> GetByPeriod(int year, int month)
    {
        return await ExecuteServiceAsync(async () =>
        {
            var period = await service.GetByPeriodAsync(year, month);
            return period ?? throw new Exception("Periodo no encontrado"); // SbcControllerBase maneja excepciones
        });
    }

    [HttpPost("close")]
    public async Task<ActionResult<AccountingPeriodDto>> ClosePeriod([FromBody] ClosePeriodRequest request)
    {
        return await ExecuteServiceAsync(
            () => service.ClosePeriodAsync(request.Year, request.Month, request.EquityAccountId), 
            HttpStatusCode.Created);
    }
}
