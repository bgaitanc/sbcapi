using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<bool> ExistsByCodeAsync(string code);
}
