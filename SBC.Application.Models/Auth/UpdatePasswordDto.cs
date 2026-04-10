namespace SBC.Application.Models.Auth;

public record UpdatePasswordDto(
    string CurrentPassword,
    string NewPassword
);
