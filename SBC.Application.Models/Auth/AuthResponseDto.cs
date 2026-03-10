namespace SBC.Application.Models.Auth;

public record AuthResponseDto(
    Guid UserId,
    string UserName,
    string Email,
    string Token,
    string RefreshToken,
    IEnumerable<string> Roles
);