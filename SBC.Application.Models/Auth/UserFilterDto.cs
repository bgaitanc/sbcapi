using SBC.Application.Models.Common;

namespace SBC.Application.Models.Auth;

public class UserFilterDto : BaseFilterDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
