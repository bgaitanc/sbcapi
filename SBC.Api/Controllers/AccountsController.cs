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
public class AccountsController : SbcControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
    {
        return await ExecuteServiceAsync(() => _accountService.GetAllAsync(), HttpStatusCode.OK, TransactionActions.GetAccounts, nameof(Account));
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetTree()
    {
        return await ExecuteServiceAsync(() => _accountService.GetTreeAsync(), HttpStatusCode.OK, TransactionActions.GetAccounts, nameof(Account), new { Type = "Tree" });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> GetById(Guid id)
    {
        return await ExecuteServiceAsync(async () =>
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                throw new SBC.Domain.Exceptions.SbcException(HttpStatusCode.NotFound, "Cuenta no encontrada.");
            }
            return account;
        }, HttpStatusCode.OK, TransactionActions.GetAccounts, nameof(Account), new { id });
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto createDto)
    {
        return await ExecuteServiceAsync(() => _accountService.CreateAsync(createDto), HttpStatusCode.Created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Update(Guid id, [FromBody] UpdateAccountDto updateDto)
    {
        return await ExecuteServiceAsync(() => _accountService.UpdateAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Delete(Guid id)
    {
        return await ExecuteServiceAsync(() => _accountService.DeleteAsync(id));
    }
}
