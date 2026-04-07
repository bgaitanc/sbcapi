using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Common;
using SBC.Application.Models.Logging;
using SBC.Application.Services.Interfaces;

using SBC.Domain.Entities.Logging;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionLogsController(ITransactionLogService logService) : SbcControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TransactionLogDto>>> GetLogs([FromQuery] TransactionLogFilterDto filter)
    {
        return await ExecuteServiceAsync(() => logService.GetPagedAsync(filter), HttpStatusCode.OK, TransactionActions.GetLogs, nameof(TransactionLog), filter);
    }
}
