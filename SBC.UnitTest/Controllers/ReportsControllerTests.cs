using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class ReportsControllerTests
{
    private readonly Mock<IFinancialReportService> _serviceMock = new();
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _controller = new ReportsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetIncomeStatement_ShouldReturnOk()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);
        var incomeStatement = new IncomeStatementDto { StartDate = startDate, EndDate = endDate, TotalRevenues = 1000 };
        _serviceMock.Setup(s => s.GetIncomeStatementAsync(startDate, endDate)).ReturnsAsync(incomeStatement);

        // Act
        var result = await _controller.GetIncomeStatement(startDate, endDate);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IncomeStatementDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<IncomeStatementDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(incomeStatement, response.Data);
    }

    [Fact]
    public async Task GetBalanceSheet_ShouldReturnOk()
    {
        // Arrange
        var date = new DateTime(2026, 1, 31);
        var balanceSheet = new BalanceSheetDto { Date = date, TotalAssets = 5000 };
        _serviceMock.Setup(s => s.GetBalanceSheetAsync(date)).ReturnsAsync(balanceSheet);

        // Act
        var result = await _controller.GetBalanceSheet(date);

        // Assert
        var actionResult = Assert.IsType<ActionResult<BalanceSheetDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<BalanceSheetDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(balanceSheet, response.Data);
    }
}
