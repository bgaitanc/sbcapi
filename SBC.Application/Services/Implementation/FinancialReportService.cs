using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.Application.Services.Implementation;

public class FinancialReportService(IJournalEntryRepository journalEntryRepository) : IFinancialReportService
{
    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate)
    {
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var report = new IncomeStatementDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        // Revenues
        var revenueLines = allLines
            .Where(l => l.Account.Type == AccountType.Revenue)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Revenues = revenueLines;
        report.TotalRevenues = revenueLines.Sum(r => r.Amount);

        // Costs
        var costLines = allLines
            .Where(l => l.Account.Type == AccountType.Cost)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Costs = costLines;
        report.TotalCosts = costLines.Sum(c => c.Amount);

        // Expenses
        var expenseLines = allLines
            .Where(l => l.Account.Type == AccountType.Expense)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Expenses = expenseLines;
        report.TotalExpenses = expenseLines.Sum(e => e.Amount);

        return report;
    }
}
