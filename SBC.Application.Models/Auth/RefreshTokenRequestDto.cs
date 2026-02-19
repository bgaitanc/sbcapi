namespace SBC.Application.Models.Auth;

public record RefreshTokenRequestDto(
    string Token,
    string RefreshToken
);