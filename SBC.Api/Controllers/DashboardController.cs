using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Dashboard;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Logging;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService service) : SbcControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        return await ExecuteServiceAsync(() => service.GetSummaryAsync(), HttpStatusCode.OK, TransactionActions.GetDashboardData, "DashboardSummary");
    }
}
