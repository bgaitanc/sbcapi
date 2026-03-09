using Moq;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest;

public class FinancialReportServiceTests
{
    [Fact]
    public async Task GetIncomeStatementAsync_ShouldCalculateCorrectly()
    {
        // Arrange
        var mockRepo = new Mock<IJournalEntryRepository>();

        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var revenueAccount = new Account
            { Id = Guid.NewGuid(), Code = "4.1.01", Name = "Ventas", Type = AccountType.Revenue };
        var costAccount = new Account
            { Id = Guid.NewGuid(), Code = "5.1.01", Name = "Costo de Ventas", Type = AccountType.Cost };
        var expenseAccount = new Account
            { Id = Guid.NewGuid(), Code = "6.1.01", Name = "Gastos Administrativos", Type = AccountType.Expense };

        var entries = new List<JournalEntry>
        {
            new()
            {
                Date = new DateTime(2026, 1, 15),
                IsPosted = true,
                Lines = new List<JournalEntryLine>
                {
                    new() { AccountId = revenueAccount.Id, Account = revenueAccount, Debit = 0, Credit = 1000 },
                    new() { AccountId = costAccount.Id, Account = costAccount, Debit = 600, Credit = 0 }
                }
            },
            new()
            {
                Date = new DateTime(2026, 1, 20),
                IsPosted = true,
                Lines = new List<JournalEntryLine>
                {
                    new() { AccountId = expenseAccount.Id, Account = expenseAccount, Debit = 100, Credit = 0 },
                    new()
                    {
                        AccountId = Guid.NewGuid(), Account = new Account { Type = AccountType.Asset }, Debit = 0,
                        Credit = 100
                    } // Contrapartida
                }
            }
        };

        mockRepo.Setup(r => r.GetByDateRangeWithLinesAsync(startDate, endDate))
            .ReturnsAsync(entries);

        var service = new FinancialReportService(mockRepo.Object);

        // Act
        var result = await service.GetIncomeStatementAsync(startDate, endDate);

        // Assert
        Assert.Equal(1000, result.TotalRevenues);
        Assert.Equal(600, result.TotalCosts);
        Assert.Equal(400, result.GrossProfit);
        Assert.Equal(100, result.TotalExpenses);
        Assert.Equal(300, result.NetIncome);

        Assert.Single(result.Revenues);
        Assert.Equal("Ventas", result.Revenues[0].AccountName);
        Assert.Equal(1000, result.Revenues[0].Amount);
    }

    [Fact]
    public async Task GetBalanceSheetAsync_ShouldCalculateCorrectly()
    {
        // Arrange
        var mockRepo = new Mock<IJournalEntryRepository>();
        var date = new DateTime(2026, 12, 31);

        var assetAccount = new Account
            { Id = Guid.NewGuid(), Code = "1.1.01", Name = "Caja", Type = AccountType.Asset };
        var liabilityAccount = new Account
            { Id = Guid.NewGuid(), Code = "2.1.01", Name = "Cuentas por Pagar", Type = AccountType.Liability };
        var equityAccount = new Account
            { Id = Guid.NewGuid(), Code = "3.1.01", Name = "Capital Social", Type = AccountType.Equity };
        var revenueAccount = new Account
            { Id = Guid.NewGuid(), Code = "4.1.01", Name = "Ventas", Type = AccountType.Revenue };

        var entries = new List<JournalEntry>
        {
            new()
            {
                Date = new DateTime(2026, 1, 1),
                Lines = new List<JournalEntryLine>
                {
                    new() { AccountId = assetAccount.Id, Account = assetAccount, Debit = 5000, Credit = 0 },
                    new() { AccountId = equityAccount.Id, Account = equityAccount, Debit = 0, Credit = 5000 }
                }
            },
            new()
            {
                Date = new DateTime(2026, 6, 1),
                Lines = new List<JournalEntryLine>
                {
                    new() { AccountId = assetAccount.Id, Account = assetAccount, Debit = 2000, Credit = 0 },
                    new() { AccountId = revenueAccount.Id, Account = revenueAccount, Debit = 0, Credit = 2000 }
                }
            },
            new()
            {
                Date = new DateTime(2026, 12, 1),
                Lines = new List<JournalEntryLine>
                {
                    new() { AccountId = assetAccount.Id, Account = assetAccount, Debit = 500, Credit = 0 },
                    new() { AccountId = liabilityAccount.Id, Account = liabilityAccount, Debit = 0, Credit = 500 }
                }
            }
        };

        mockRepo.Setup(r => r.GetByDateRangeWithLinesAsync(DateTime.MinValue, date))
            .ReturnsAsync(entries);

        var service = new FinancialReportService(mockRepo.Object);

        // Act
        var result = await service.GetBalanceSheetAsync(date);

        // Assert
        Assert.Equal(7500, result.TotalAssets);
        Assert.Equal(500, result.TotalLiabilities);
        Assert.Equal(5000, result.TotalEquity);
        Assert.Equal(2000, result.NetIncome);
        Assert.Equal(7500, result.TotalLiabilitiesAndEquity);

        Assert.Single(result.Assets);
        Assert.Equal("Caja", result.Assets[0].AccountName);
        Assert.Equal(7500, result.Assets[0].Amount);
    }
}