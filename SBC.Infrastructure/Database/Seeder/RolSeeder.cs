using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SBC.Infrastructure.Database.Seeder;

public static class RolSeeder
{
    public static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("8bc2bfc1-1e12-4b5f-a1ff-679b06b1996b"), Name = "Admin", NormalizedName = "ADMIN",
                ConcurrencyStamp = "c735af80-37c2-434b-9c50-c8f3c5fd1be2"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("15babe3a-44b5-4045-81b2-d1e4ef87ce32"), Name = "Guest", NormalizedName = "GUEST",
                ConcurrencyStamp = "5150d7aa-213d-42a4-945e-5d60c96c6485"
            }
        );
    }
}