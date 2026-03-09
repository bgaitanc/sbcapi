using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

public interface IAccountingPeriodService
{
    Task<AccountingPeriodDto> ClosePeriodAsync(int year, int month, Guid equityAccountId);
    Task<IEnumerable<AccountingPeriodDto>> GetAllPeriodsAsync();
    Task<AccountingPeriodDto?> GetByPeriodAsync(int year, int month);
}
