using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto> CreateAsync(CreateAccountDto createDto);
    Task UpdateAsync(Guid id, UpdateAccountDto updateDto);
    Task DeleteAsync(Guid id);
}
