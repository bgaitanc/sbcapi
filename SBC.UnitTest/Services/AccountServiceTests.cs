using System.Net;
using Moq;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest.Services;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock = new();
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _service = new AccountService(_accountRepoMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenAccountExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = new Account { Id = id, Code = "1.1", Name = "Caja", Type = AccountType.Asset };
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(account);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
        Assert.Equal(account.Code, result.Code);
        Assert.Equal(account.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), Code = "1.1", Name = "Caja", Type = AccountType.Asset },
            new() { Id = Guid.NewGuid(), Code = "1.2", Name = "Bancos", Type = AccountType.Asset }
        };
        _accountRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateAccountDto { Code = "1.1", Name = "Caja", Type = AccountType.Asset };
        _accountRepoMock.Setup(r => r.ExistsByCodeAsync(createDto.Code)).ReturnsAsync(false);
        _accountRepoMock.Setup(r => r.AddAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) => a);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Code, result.Code);
        Assert.Equal(createDto.Name, result.Name);
        _accountRepoMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenCodeAlreadyExists()
    {
        // Arrange
        var createDto = new CreateAccountDto { Code = "1.1", Name = "Caja", Type = AccountType.Asset };
        _accountRepoMock.Setup(r => r.ExistsByCodeAsync(createDto.Code)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.CreateAsync(createDto));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateAccountDto { Code = "1.1.1", Name = "Caja Chica", Type = AccountType.Asset };
        var existingAccount = new Account { Id = id, Code = "1.1", Name = "Caja", Type = AccountType.Asset };

        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingAccount);
        _accountRepoMock.Setup(r => r.ExistsByCodeAsync(updateDto.Code)).ReturnsAsync(false);

        // Act
        await _service.UpdateAsync(id, updateDto);

        // Assert
        _accountRepoMock.Verify(r => r.UpdateAsync(It.Is<Account>(a => a.Code == updateDto.Code && a.Name == updateDto.Name)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenAccountNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateAccountDto { Code = "1.1.1", Name = "Caja Chica", Type = AccountType.Asset };
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.UpdateAsync(id, updateDto));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenNewCodeExistsInAnotherAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateAccountDto { Code = "1.1.1", Name = "Caja Chica", Type = AccountType.Asset };
        var existingAccount = new Account { Id = id, Code = "1.1", Name = "Caja", Type = AccountType.Asset };

        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingAccount);
        _accountRepoMock.Setup(r => r.ExistsByCodeAsync(updateDto.Code)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.UpdateAsync(id, updateDto));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingAccount = new Account { Id = id, Code = "1.1", Name = "Caja", Type = AccountType.Asset };
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingAccount);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _accountRepoMock.Verify(r => r.DeleteAsync(existingAccount), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenAccountNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.DeleteAsync(id));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenAccountHasSubAccounts()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingAccount = new Account 
        { 
            Id = id, 
            Code = "1.1", 
            Name = "Caja", 
            Type = AccountType.Asset,
            SubAccounts = new List<Account> { new() { Id = Guid.NewGuid() } }
        };
        _accountRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingAccount);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.DeleteAsync(id));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }
}
