using System.Net;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.Application.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account == null ? null : MapToDto(account);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(MapToDto);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto createDto)
    {
        if (await _accountRepository.ExistsByCodeAsync(createDto.Code))
        {
            throw new SbcException(HttpStatusCode.BadRequest, $"La cuenta con código {createDto.Code} ya existe.");
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Code = createDto.Code,
            Name = createDto.Name,
            Type = createDto.Type,
            ParentAccountId = createDto.ParentAccountId
        };

        var createdAccount = await _accountRepository.AddAsync(account);
        return MapToDto(createdAccount);
    }

    public async Task UpdateAsync(Guid id, UpdateAccountDto updateDto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
        {
            throw new SbcException(HttpStatusCode.NotFound, "Cuenta no encontrada.");
        }

        if (account.Code != updateDto.Code && await _accountRepository.ExistsByCodeAsync(updateDto.Code))
        {
            throw new SbcException(HttpStatusCode.BadRequest, $"La cuenta con código {updateDto.Code} ya existe.");
        }

        account.Code = updateDto.Code;
        account.Name = updateDto.Name;
        account.Type = updateDto.Type;
        account.ParentAccountId = updateDto.ParentAccountId;

        await _accountRepository.UpdateAsync(account);
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
        {
            throw new SbcException(HttpStatusCode.NotFound, "Cuenta no encontrada.");
        }

        if (account.SubAccounts != null && account.SubAccounts.Any())
        {
            throw new SbcException(HttpStatusCode.BadRequest, "No se puede eliminar una cuenta que tiene subcuentas.");
        }

        await _accountRepository.DeleteAsync(account);
    }

    private static AccountDto MapToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            Type = account.Type,
            ParentAccountId = account.ParentAccountId,
            ParentAccountName = account.ParentAccount?.Name,
            CreatedAt = account.CreatedAt,
            CreatedBy = account.CreatedBy,
            UpdatedAt = account.UpdatedAt,
            UpdatedBy = account.UpdatedBy
        };
    }
}
