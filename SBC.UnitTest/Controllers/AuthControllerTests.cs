using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _serviceMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_serviceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@example.com", "Password123!", "John", "Doe");
        var userId = Guid.NewGuid();
        _serviceMock.Setup(s => s.RegisterUserAsync(registerDto)).ReturnsAsync(userId);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Guid>>(result);
        var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<Guid>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(userId, response.Data);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnOk()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "Password123!");
        var authResponse = new AuthResponseDto(Guid.NewGuid(), "test", "test@example.com", "token", "refresh",
            new List<string> { "Guest" });
        _serviceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AuthResponseDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AuthResponseDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(authResponse, response.Data);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk()
    {
        // Arrange
        var request = new RefreshTokenRequestDto("old_token", "refresh_token");
        var authResponse = new AuthResponseDto(Guid.NewGuid(), "test", "test@example.com", "new_token", "new_refresh",
            new List<string> { "Guest" });
        _serviceMock.Setup(s => s.RefreshTokenAsync(request)).ReturnsAsync(authResponse);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<AuthResponseDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<AuthResponseDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(authResponse, response.Data);
    }
}