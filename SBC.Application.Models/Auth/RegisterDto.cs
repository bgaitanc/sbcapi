namespace SBC.Application.Models.Auth;

public record RegisterDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
);