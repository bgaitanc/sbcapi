using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Base;
using SBC.Domain.Entities.Identity;
using SBC.Infrastructure.Database.Seeder;

namespace SBC.Infrastructure.Database;

public class SbcDbContext(DbContextOptions<SbcDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
    public DbSet<AccountingPeriod> AccountingPeriods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SbcDbContext).Assembly);

        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "identity");
        modelBuilder.Entity<ApplicationUser>().Property(x => x.FirstName).IsRequired().HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        modelBuilder.Entity<ApplicationUser>().Property(x => x.LastName).IsRequired().HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", "identity");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "identity");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "identity");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "identity");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "identity");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "identity");

        RolSeeder.SeedRoles(modelBuilder);
        AccountSeeder.SeedAccounts(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var trackedEntities = GetTrackedEntities();

        var now = DateTime.Now;
        foreach (var entityEntry in trackedEntities)
        {
            if (entityEntry.Entity is not BaseEntity entity) continue;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                Entry(entity).Property(x => x.CreatedAt).IsModified = true;
                Entry(entity).Property(x => x.UpdatedAt).IsModified = false;
                Entry(entity).Property(x => x.UpdatedBy).IsModified = false;
            }

            if (entityEntry.State != EntityState.Modified) continue;

            entity.UpdatedAt = now;
            Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
            Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            Entry(entity).Property(x => x.CreatedBy).IsModified = false;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private IEnumerable<EntityEntry> GetTrackedEntities()
    {
        var state = new List<EntityState>
        {
            EntityState.Added,
            EntityState.Modified
        };

        return ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && state.Any(s => e.State == s));
    }
}