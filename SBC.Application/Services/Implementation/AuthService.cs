using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SBC.Application.Models.Auth;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Identity;
using SBC.Domain.Exceptions;

namespace SBC.Application.Services.Implementation;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration)
    : IAuthService
{
    public async Task<Guid> RegisterUserAsync(RegisterDto registerDto)
    {
        var emailExists = await userManager.FindByEmailAsync(registerDto.Email);
        if (emailExists != null)
            throw new SbcException(HttpStatusCode.PreconditionFailed,
                "El correo electrónico ya está registrado.");

        var userNameExists = await userManager.FindByNameAsync(registerDto.UserName);
        if (userNameExists != null)
            throw new SbcException(HttpStatusCode.PreconditionFailed,
                "El nombre de usuario ya está registrado.");

        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new SbcException(HttpStatusCode.PreconditionFailed, $"Error al registrar usuario: {errors}");
        }

        await userManager.AddToRoleAsync(user, "Guest");

        return user.Id;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new SbcException(HttpStatusCode.Unauthorized, "Credenciales inválidas.");
        }

        var accessToken = await GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        await UpdateUserRefreshToken(user, refreshToken);

        var roles = await userManager.GetRolesAsync(user);

        return new AuthResponseDto(
            UserName: user.UserName!,
            UserId: user.Id,
            Email: user.Email!,
            Token: accessToken,
            RefreshToken: refreshToken,
            Roles: roles
        );
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            throw new SbcException(HttpStatusCode.Unauthorized, "Token inválido.");

        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) throw new SbcException(HttpStatusCode.Unauthorized, "Token inválido.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new SbcException(HttpStatusCode.Unauthorized, "Refresh token inválido o expirado.");
        }

        var newAccessToken = await GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        await UpdateUserRefreshToken(user, newRefreshToken);

        var roles = await userManager.GetRolesAsync(user);

        return new AuthResponseDto(
            UserName: user.UserName!,
            UserId: user.Id,
            Email: user.Email!,
            Token: newAccessToken,
            RefreshToken: newRefreshToken,
            Roles: roles
        );
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
            throw new SbcException(HttpStatusCode.BadRequest, "La clave secreta JWT no está configurada.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var audience = jwtSettings["Audience"];
        var issuer = jwtSettings["Issuer"];

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidAudience = audience,
            ValidIssuer = issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private async Task UpdateUserRefreshToken(ApplicationUser user, string refreshToken)
    {
        user.RefreshToken = refreshToken;

        var expiryTime = DateTime.UtcNow.AddDays(7);
        user.RefreshTokenExpiryTime = DateTime.SpecifyKind(expiryTime, DateTimeKind.Unspecified);

        await userManager.UpdateAsync(user);
    }
}