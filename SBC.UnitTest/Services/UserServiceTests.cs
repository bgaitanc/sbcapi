using System.Net;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using SBC.Application.Models.Auth;
using SBC.Application.Models.Common;
using SBC.Application.Services.Implementation;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Identity;
using SBC.Domain.Exceptions;

namespace SBC.UnitTest.Services;

public class UserServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITransactionLogService> _transactionLogServiceMock = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _service = new UserService(_userManagerMock.Object, _transactionLogServiceMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnUserId_WhenSuccessfulWithRoles()
    {
        // Arrange
        var roles = new List<string> { "Admin", "Manager" };
        var createUserDto = new CreateUserDto("newuser", "new@example.com", "Password123!", "Jane", "Doe", roles);
        
        _userManagerMock.Setup(m => m.FindByEmailAsync(createUserDto.Email)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.FindByNameAsync(createUserDto.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password))
            .Callback<ApplicationUser, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.CreateUserAsync(createUserDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _userManagerMock.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => u.Email == createUserDto.Email), createUserDto.Password), Times.Once);
        _userManagerMock.Verify(m => m.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAssignGuestRole_WhenNoRolesProvided()
    {
        // Arrange
        var createUserDto = new CreateUserDto("guestuser", "guest@example.com", "Password123!", "Guest", "User", new List<string>());
        
        _userManagerMock.Setup(m => m.FindByEmailAsync(createUserDto.Email)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.FindByNameAsync(createUserDto.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), createUserDto.Password))
            .Callback<ApplicationUser, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Guest"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.CreateUserAsync(createUserDto);

        // Assert
        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Guest"), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDto("user", "exists@example.com", "Pass123!", "F", "L", new List<string>());
        _userManagerMock.Setup(m => m.FindByEmailAsync(createUserDto.Email)).ReturnsAsync(new ApplicationUser { FirstName = "Ex", LastName = "Is" });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.CreateUserAsync(createUserDto));
        Assert.Equal(HttpStatusCode.PreconditionFailed, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUserSuccessfully_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, UserName = "old", Email = "old@test.com", FirstName = "O", LastName = "L" };
        var updateUserDto = new UpdateUserDto("new", "new@test.com", "N", "E", new List<string> { "Manager" });

        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.FindByEmailAsync(updateUserDto.Email)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.FindByNameAsync(updateUserDto.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Guest" });
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRolesAsync(user, updateUserDto.Roles)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.UpdateUserAsync(userId, updateUserDto);

        // Assert
        Assert.Equal(updateUserDto.UserName, user.UserName);
        Assert.Equal(updateUserDto.Email, user.Email);
        _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _userManagerMock.Verify(m => m.AddToRolesAsync(user, updateUserDto.Roles), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.UpdateUserAsync(userId, new UpdateUserDto("", "", "", "", [])));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldChangePasswordSuccessfully_WhenCredentialsAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, FirstName = "F", LastName = "L" };
        var updatePasswordDto = new UpdatePasswordDto("OldPass123!", "NewPass123!");

        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.UpdatePasswordAsync(userId, updatePasswordDto);

        // Assert
        _userManagerMock.Verify(m => m.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword), Times.Once);
    }

    [Fact]
    public async Task GetPagedUsersAsync_ShouldReturnPagedResults_WhenFiltering()
    {
        // Arrange
        var userList = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "admin1", Email = "admin1@test.com", FirstName = "Admin", LastName = "One" },
            new() { Id = Guid.NewGuid(), UserName = "user1", Email = "user1@test.com", FirstName = "User", LastName = "One" },
            new() { Id = Guid.NewGuid(), UserName = "admin2", Email = "admin2@test.com", FirstName = "Admin", LastName = "Two" }
        };

        var mock = userList.AsQueryable().BuildMock();
        _userManagerMock.Setup(m => m.Users).Returns(mock);
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Guest" });

        var filter = new UserFilterDto { UserName = "admin", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetPagedUsersAsync(filter);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, u => Assert.Contains("admin", u.UserName));
    }

    [Fact]
    public async Task GetPagedUsersAsync_ShouldReturnCorrectPage_WhenPaging()
    {
        // Arrange
        var userList = new List<ApplicationUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "user1", Email = "u1@test.com", FirstName = "U1", LastName = "L1" },
            new() { Id = Guid.NewGuid(), UserName = "user2", Email = "u2@test.com", FirstName = "U2", LastName = "L2" },
            new() { Id = Guid.NewGuid(), UserName = "user3", Email = "u3@test.com", FirstName = "U3", LastName = "L3" }
        };

        var mock = userList.AsQueryable().BuildMock();
        _userManagerMock.Setup(m => m.Users).Returns(mock);
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Guest" });

        var filter = new UserFilterDto { PageNumber = 2, PageSize = 1 };

        // Act
        var result = await _service.GetPagedUsersAsync(filter);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal("user2", result.Items.First().UserName);
    }
}
