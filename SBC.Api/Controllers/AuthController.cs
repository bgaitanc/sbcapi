using System.Net;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Interfaces;

namespace SBC.Api.Controllers;

/// <summary>
/// Controller responsible for authentication-related operations, such as user
/// registration, login, and refreshing tokens.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) :  SbcControllerBase
{
    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="registerDto">
    /// An object containing the user's email, password, first name, and last name.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> encapsulating an <see cref="ActionResult{Guid}"/>.
    /// The result contains the unique identifier of the newly registered user
    /// if registration is successful, along with a status code of Created (201).
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(SbcGenericResponse<Guid>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto registerDto)
    {
        return await ExecuteServiceAsync(async () => await authService.RegisterUserAsync(registerDto),
            HttpStatusCode.Created);
    }

    /// <summary>
    /// Authenticates a user and provides an authentication response containing a token,
    /// refresh token, and associated user details upon successful login.
    /// </summary>
    /// <param name="loginDto">
    /// An object containing the user's email and password used for authentication.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> encapsulating an <see cref="ActionResult{AuthResponseDto}"/>.
    /// The result contains the authentication response with user information, roles,
    /// and tokens if login is successful, along with a status code of OK (200).
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SbcGenericResponse<AuthResponseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        return await ExecuteServiceAsync(async () => await authService.LoginAsync(loginDto));
    }

    /// <summary>
    /// Refreshes the authentication token using the provided refresh token request.
    /// </summary>
    /// <param name="request">
    /// An object containing the current token and its corresponding refresh token.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> encapsulating an <see cref="ActionResult{AuthResponseDto}"/>.
    /// The result includes a new authentication token and refresh token if the operation
    /// is successful.
    /// </returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(SbcGenericResponse<AuthResponseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.PreconditionFailed)]
    [ProducesResponseType(typeof(SbcGenericResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        return await ExecuteServiceAsync(async () => await authService.RefreshTokenAsync(request));
    }
}