using System.Collections.Generic;

namespace SBC.Application.Models.Auth;

public record CreateUserDto(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    List<string> Roles
);
