using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

public interface IFinancialReportService
{
    Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate, bool includeUnposted = false);
    Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime date, bool includeUnposted = false);
}
