using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class AccountingPeriodsControllerTests
{
    private readonly Mock<IAccountingPeriodService> _serviceMock = new();
    private readonly AccountingPeriodsController _controller;

    public AccountingPeriodsControllerTests()
    {
        _controller = new AccountingPeriodsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var periods = new List<AccountingPeriodDto> { new() { Id = Guid.NewGuid(), Year = 2026, Month = 1 } };
        _serviceMock.Setup(s => s.GetAllPeriodsAsync()).ReturnsAsync(periods);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<AccountingPeriodDto>>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<IEnumerable<AccountingPeriodDto>>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(periods, response.Data);
    }

    [Fact]
    public async Task GetByPeriod_WhenExists_ShouldReturnOk()
    {
        // Arrange
        int year = 2026, month = 1;
        var period = new AccountingPeriodDto { Id = Guid.NewGuid(), Year = year, Month = month };
        _serviceMock.Setup(s => s.GetByPeriodAsync(year, month)).ReturnsAsync(period);

        // Act
        var result = await _controller.GetByPeriod(year, month);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AccountingPeriodDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AccountingPeriodDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(period, response.Data);
    }

    [Fact]
    public async Task ClosePeriod_ShouldReturnCreated()
    {
        // Arrange
        var request = new ClosePeriodRequest { Year = 2026, Month = 1, EquityAccountId = Guid.NewGuid() };
        var closedPeriod = new AccountingPeriodDto { Id = Guid.NewGuid(), Year = request.Year, Month = request.Month, IsClosed = true };
        _serviceMock.Setup(s => s.ClosePeriodAsync(request.Year, request.Month, request.EquityAccountId)).ReturnsAsync(closedPeriod);

        // Act
        var result = await _controller.ClosePeriod(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AccountingPeriodDto>>(result);
        var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AccountingPeriodDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
