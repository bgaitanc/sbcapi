using System;
using System.Collections.Generic;

namespace SBC.Application.Models.Auth;

public record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles
);
