using SBC.Application.Models.Dashboard;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.Application.Services.Implementation;

public class DashboardService(
    IJournalEntryRepository journalEntryRepository,
    IJournalEntryLineRepository journalEntryLineRepository) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var now = DateTime.Now;
        // Obtenemos todos los movimientos hasta ahora, incluyendo no mayorizados para el "en vivo"
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(DateTime.MinValue, now, true);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var summary = new DashboardSummaryDto
        {
            TotalAssets = allLines.Where(l => l.Account.Type == AccountType.Asset).Sum(l => l.Debit - l.Credit),
            TotalLiabilities = allLines.Where(l => l.Account.Type == AccountType.Liability).Sum(l => l.Credit - l.Debit),
            TotalRevenue = allLines.Where(l => l.Account.Type == AccountType.Revenue).Sum(l => l.Credit - l.Debit),
            TotalCosts = allLines.Where(l => l.Account.Type == AccountType.Cost).Sum(l => l.Debit - l.Credit),
            TotalExpenses = allLines.Where(l => l.Account.Type == AccountType.Expense).Sum(l => l.Debit - l.Credit)
        };

        summary.TotalEquity = allLines.Where(l => l.Account.Type == AccountType.Equity).Sum(l => l.Credit - l.Debit);
        summary.NetIncome = summary.TotalRevenue - summary.TotalCosts - summary.TotalExpenses;

        // Ultimos movimientos
        var recentEntries = await journalEntryRepository.GetRecentEntriesAsync(10);
        summary.RecentMovements = recentEntries.Select(e => new RecentMovementDto
        {
            Id = e.Id,
            Code = e.Code,
            Date = e.Date,
            Description = e.Description,
            TotalAmount = e.Lines.Sum(l => l.Debit)
        }).ToList();

        // Cuentas con mas movimientos
        var topAccounts = await journalEntryLineRepository.GetTopAccountsAsync(5);
        summary.TopAccounts = topAccounts.Select(a => new TopAccountDto
        {
            AccountId = a.AccountId,
            AccountCode = a.Code,
            AccountName = a.Name,
            MovementCount = a.Count,
            TotalAmount = a.Total
        }).ToList();

        return summary;
    }
}
