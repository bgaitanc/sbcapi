using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Identity;

namespace SBC.Infrastructure.Database.Seeder;

public static class UserSeeder
{
    public static void SeedUsers(ModelBuilder builder)
    {
        var adminRoleId = Guid.Parse("8bc2bfc1-1e12-4b5f-a1ff-679b06b1996b");
        var adminUserId = Guid.Parse("01c0b3b4-f06b-4e1b-852d-94863c87f6e1");

        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@sbc.com",
            NormalizedEmail = "ADMIN@SBC.COM",
            EmailConfirmed = true,
            FirstName = "Administrador",
            LastName = "SBC",
            SecurityStamp = "52705f07-a77e-41cb-aadf-1f549e91434a",
            ConcurrencyStamp = "8dce09d9-ddfe-4cda-bc89-450169fab180",
            PasswordHash = "AQAAAAIAAYagAAAAEMbXUX7hqw4CNdCZ6tHC/tlOeuSJG3f/gKR0a/7zhorqe66hRzMzSF1iEH8LWtkWoQ=="
        };

        builder.Entity<ApplicationUser>().HasData(adminUser);

        builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
        {
            RoleId = adminRoleId,
            UserId = adminUserId
        });
    }
}
