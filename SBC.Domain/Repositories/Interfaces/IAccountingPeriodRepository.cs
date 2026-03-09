using SBC.Domain.Entities.Accounting;

namespace SBC.Domain.Repositories.Interfaces;

public interface IAccountingPeriodRepository : IBaseRepository<AccountingPeriod>
{
    Task<AccountingPeriod?> GetByPeriodAsync(int year, int month);
    Task<bool> IsPeriodClosedAsync(int year, int month);
}
