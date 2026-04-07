using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class FinancialReportService(
    IJournalEntryRepository journalEntryRepository,
    ITransactionLogService transactionLogService) : IFinancialReportService
{
    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate, bool includeUnposted = false)
    {
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate, includeUnposted);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var report = new IncomeStatementDto
        {
            StartDate = startDate,
            EndDate = endDate,
            IsProvisional = includeUnposted
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

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GenerateIncomeStatement, nameof(IncomeStatementDto), null, TransactionStatus.Success, JsonSerializer.Serialize(new { startDate, endDate, includeUnposted }));

        return report;
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime date, bool includeUnposted = false)
    {
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(DateTime.MinValue, date, includeUnposted);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var report = new BalanceSheetDto
        {
            Date = date,
            IsProvisional = includeUnposted
        };

        // Assets
        var assetLines = allLines
            .Where(l => l.Account.Type == AccountType.Asset)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Assets = assetLines;
        report.TotalAssets = assetLines.Sum(a => a.Amount);

        // Liabilities
        var liabilityLines = allLines
            .Where(l => l.Account.Type == AccountType.Liability)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Liabilities = liabilityLines;
        report.TotalLiabilities = liabilityLines.Sum(l => l.Amount);

        // Equity
        var equityLines = allLines
            .Where(l => l.Account.Type == AccountType.Equity)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Equity = equityLines;
        report.TotalEquity = equityLines.Sum(e => e.Amount);

        // Net Income (Historical)
        var revenue = allLines.Where(l => l.Account.Type == AccountType.Revenue).Sum(l => l.Credit - l.Debit);
        var costs = allLines.Where(l => l.Account.Type == AccountType.Cost).Sum(l => l.Debit - l.Credit);
        var expenses = allLines.Where(l => l.Account.Type == AccountType.Expense).Sum(l => l.Debit - l.Credit);
        report.NetIncome = revenue - costs - expenses;

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GenerateBalanceSheet, nameof(BalanceSheetDto), null, TransactionStatus.Success, JsonSerializer.Serialize(new { date, includeUnposted }));

        return report;
    }
}
