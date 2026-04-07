using System.Net;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionLogService _transactionLogService;

    public AccountService(IAccountRepository accountRepository, ITransactionLogService transactionLogService)
    {
        _accountRepository = accountRepository;
        _transactionLogService = transactionLogService;
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

    public async Task<IEnumerable<AccountDto>> GetTreeAsync()
    {
        // Traemos todas las cuentas para asegurar que podemos construir el árbol completo en memoria
        // incluso si EF no cargó todos los niveles recursivamente con Include.
        var allAccounts = await _accountRepository.GetAllAsync();
        var allDtos = allAccounts.Select(MapToDto).ToList();

        var roots = allDtos.Where(a => a.ParentAccountId == null).OrderBy(a => a.Code).ToList();
        var childrenMap = allDtos.Where(a => a.ParentAccountId != null)
            .GroupBy(a => a.ParentAccountId)
            .ToDictionary(g => g.Key!.Value, g => g.OrderBy(a => a.Code).ToList());

        foreach (var dto in allDtos)
        {
            if (childrenMap.TryGetValue(dto.Id, out var children))
            {
                dto.Children = children;
            }
        }

        return roots;
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto createDto)
    {
        if (await _accountRepository.ExistsByCodeAsync(createDto.Code))
        {
            var error = $"La cuenta con código {createDto.Code} ya existe.";
            await _transactionLogService.LogTransactionAsync(null, "CreateAccount", "Account", null, "ValidationError", JsonSerializer.Serialize(createDto), error);
            throw new SbcException(HttpStatusCode.BadRequest, error);
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
        await _transactionLogService.LogTransactionAsync(null, "CreateAccount", "Account", createdAccount.Id.ToString(), "Success", JsonSerializer.Serialize(createDto));
        return MapToDto(createdAccount);
    }

    public async Task UpdateAsync(Guid id, UpdateAccountDto updateDto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
        {
            var error = "Cuenta no encontrada.";
            await _transactionLogService.LogTransactionAsync(null, "UpdateAccount", "Account", id.ToString(), "Failure", JsonSerializer.Serialize(updateDto), error);
            throw new SbcException(HttpStatusCode.NotFound, error);
        }

        if (account.Code != updateDto.Code && await _accountRepository.ExistsByCodeAsync(updateDto.Code))
        {
            var error = $"La cuenta con código {updateDto.Code} ya existe.";
            await _transactionLogService.LogTransactionAsync(null, "UpdateAccount", "Account", id.ToString(), "ValidationError", JsonSerializer.Serialize(updateDto), error);
            throw new SbcException(HttpStatusCode.BadRequest, error);
        }

        account.Code = updateDto.Code;
        account.Name = updateDto.Name;
        account.Type = updateDto.Type;
        account.ParentAccountId = updateDto.ParentAccountId;

        await _accountRepository.UpdateAsync(account);
        await _transactionLogService.LogTransactionAsync(null, "UpdateAccount", "Account", id.ToString(), "Success", JsonSerializer.Serialize(updateDto));
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
        {
            var error = "Cuenta no encontrada.";
            await _transactionLogService.LogTransactionAsync(null, "DeleteAccount", "Account", id.ToString(), "Failure", null, error);
            throw new SbcException(HttpStatusCode.NotFound, error);
        }

        if (account.SubAccounts != null && account.SubAccounts.Any())
        {
            var error = "No se puede eliminar una cuenta que tiene subcuentas.";
            await _transactionLogService.LogTransactionAsync(null, "DeleteAccount", "Account", id.ToString(), "ValidationError", null, error);
            throw new SbcException(HttpStatusCode.BadRequest, error);
        }

        await _accountRepository.DeleteAsync(account);
        await _transactionLogService.LogTransactionAsync(null, "DeleteAccount", "Account", id.ToString(), "Success");
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
