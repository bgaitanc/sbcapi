using System.Collections.Generic;

namespace SBC.Application.Models.Auth;

public record UpdateUserDto(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles
);
