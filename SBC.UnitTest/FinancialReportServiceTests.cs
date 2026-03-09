using Moq;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Repositories.Interfaces;
using Xunit;

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
        
        var revenueAccount = new Account { Id = Guid.NewGuid(), Code = "4.1.01", Name = "Ventas", Type = AccountType.Revenue };
        var costAccount = new Account { Id = Guid.NewGuid(), Code = "5.1.01", Name = "Costo de Ventas", Type = AccountType.Cost };
        var expenseAccount = new Account { Id = Guid.NewGuid(), Code = "6.1.01", Name = "Gastos Administrativos", Type = AccountType.Expense };

        var entries = new List<JournalEntry>
        {
            new JournalEntry
            {
                Date = new DateTime(2026, 1, 15),
                IsPosted = true,
                Lines = new List<JournalEntryLine>
                {
                    new JournalEntryLine { AccountId = revenueAccount.Id, Account = revenueAccount, Debit = 0, Credit = 1000 },
                    new JournalEntryLine { AccountId = costAccount.Id, Account = costAccount, Debit = 600, Credit = 0 }
                }
            },
            new JournalEntry
            {
                Date = new DateTime(2026, 1, 20),
                IsPosted = true,
                Lines = new List<JournalEntryLine>
                {
                    new JournalEntryLine { AccountId = expenseAccount.Id, Account = expenseAccount, Debit = 100, Credit = 0 },
                    new JournalEntryLine { AccountId = Guid.NewGuid(), Account = new Account { Type = AccountType.Asset }, Debit = 0, Credit = 100 } // Contrapartida
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
}
