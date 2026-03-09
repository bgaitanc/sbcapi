using Moq;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Implementation;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest.Services;

public class AccountingPeriodServiceTests
{
    private readonly Mock<IAccountingPeriodRepository> _periodRepoMock = new();
    private readonly Mock<IJournalEntryRepository> _journalRepoMock = new();
    private readonly Mock<IFinancialReportService> _reportServiceMock = new();
    private readonly Mock<IAccountRepository> _accountRepoMock = new();
    private readonly AccountingPeriodService _service;

    public AccountingPeriodServiceTests()
    {
        _service = new AccountingPeriodService(
            _periodRepoMock.Object,
            _journalRepoMock.Object,
            _reportServiceMock.Object,
            _accountRepoMock.Object);
    }

    [Fact]
    public async Task ClosePeriodAsync_ShouldCreateClosingEntryAndMarkPeriodAsClosed()
    {
        // Arrange
        int year = 2026;
        int month = 1;
        var equityAccountId = Guid.NewGuid();
        var startDate = new DateTime(year, month, 1);
        var endDate = new DateTime(year, month, 31);

        _periodRepoMock.Setup(r => r.GetByPeriodAsync(year, month)).ReturnsAsync((AccountingPeriod)null);
        _accountRepoMock.Setup(r => r.GetByIdAsync(equityAccountId)).ReturnsAsync(new Account { Id = equityAccountId, Type = AccountType.Equity });
        
        var unpostedEntry = new JournalEntry { Date = new DateTime(2026, 1, 15), IsPosted = false };
        _journalRepoMock.Setup(r => r.GetByDateRangeWithLinesAsync(startDate, endDate))
            .ReturnsAsync(new List<JournalEntry> { unpostedEntry });

        var incomeStatement = new IncomeStatementDto
        {
            TotalRevenues = 1000,
            TotalCosts = 400,
            TotalExpenses = 100,
            Revenues = new List<IncomeStatementLineDto> { new() { AccountId = Guid.NewGuid(), Amount = 1000, AccountCode = "4.1" } },
            Costs = new List<IncomeStatementLineDto> { new() { AccountId = Guid.NewGuid(), Amount = 400, AccountCode = "5.1" } },
            Expenses = new List<IncomeStatementLineDto> { new() { AccountId = Guid.NewGuid(), Amount = 100, AccountCode = "6.1" } }
        };
        // NetIncome = 1000 - 400 - 100 = 500
        
        _reportServiceMock.Setup(s => s.GetIncomeStatementAsync(startDate, endDate))
            .ReturnsAsync(incomeStatement);

        _journalRepoMock.Setup(r => r.AddAsync(It.IsAny<JournalEntry>()))
            .ReturnsAsync((JournalEntry e) => { e.Id = Guid.NewGuid(); return e; });

        // Act
        var result = await _service.ClosePeriodAsync(year, month, equityAccountId);

        // Assert
        Assert.True(result.IsClosed);
        Assert.NotNull(result.ClosingJournalEntryId);
        
        // Verificar que se mayorizó el asiento pendiente
        _journalRepoMock.Verify(r => r.UpdateAsync(It.Is<JournalEntry>(e => e.IsPosted)), Times.Once);
        
        // Verificar que se creó el asiento de cierre con las líneas correctas
        _journalRepoMock.Verify(r => r.AddAsync(It.Is<JournalEntry>(e => 
            e.IsPosted && 
            e.Lines.Count == 4 && // 1 Rev + 1 Cost + 1 Exp + 1 Equity
            e.Lines.Any(l => l.AccountId == equityAccountId && l.Credit == 500))), Times.Once);

        _periodRepoMock.Verify(r => r.AddAsync(It.Is<AccountingPeriod>(p => p.IsClosed && p.Year == year && p.Month == month)), Times.Once);
    }
}
