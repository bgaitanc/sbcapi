using Microsoft.AspNetCore.Identity;

namespace SBC.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}