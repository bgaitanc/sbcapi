using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Identity;
using SBC.Domain.Exceptions;

namespace SBC.UnitTest.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _configMock = new Mock<IConfiguration>();
        _service = new AuthService(_userManagerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnUserId_WhenSuccessful()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@example.com", "Password123!", "John", "Doe");
        _userManagerMock.Setup(m => m.FindByEmailAsync(registerDto.Email)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.FindByNameAsync(registerDto.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .Callback<ApplicationUser, string>((user, _) => user.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Guest"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.RegisterUserAsync(registerDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _userManagerMock.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => u.Email == registerDto.Email && u.UserName == registerDto.UserName), registerDto.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@example.com", "Password123!", "John", "Doe");
        _userManagerMock.Setup(m => m.FindByEmailAsync(registerDto.Email)).ReturnsAsync(new ApplicationUser { FirstName = "John", LastName = "Doe" });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.RegisterUserAsync(registerDto));
        Assert.Equal(HttpStatusCode.PreconditionFailed, exception.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "Password123!");
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = loginDto.UserName, Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        
        _userManagerMock.Setup(m => m.FindByNameAsync(loginDto.UserName)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        _configMock.Setup(c => c.GetSection("JwtSettings")["SecretKey"]).Returns("a_very_long_secret_key_with_at_least_32_characters");

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Email, result.Email);
        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "WrongPassword");
        _userManagerMock.Setup(m => m.FindByNameAsync(loginDto.UserName)).ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SbcException>(() => _service.LoginAsync(loginDto));
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }
}
