using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IAccountService> _serviceMock = new();
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _controller = new AccountsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var accounts = new List<AccountDto> { new() { Id = Guid.NewGuid(), Name = "Test Account" } };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<AccountDto>>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<IEnumerable<AccountDto>>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(accounts, response.Data);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = new AccountDto { Id = id, Name = "Test Account" };
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(account);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AccountDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AccountDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(account, response.Data);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateAccountDto { Name = "New Account", Code = "1.1" };
        var createdAccount = new AccountDto { Id = Guid.NewGuid(), Name = createDto.Name, Code = createDto.Code };
        _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdAccount);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AccountDto>>(result);
        var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AccountDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateAccountDto { Name = "Updated Account", Code = "1.1" };
        _serviceMock.Setup(s => s.UpdateAsync(id, updateDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, updateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
        Assert.IsType<NoContentResult>(actionResult.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
        Assert.IsType<NoContentResult>(actionResult.Result);
    }
}
