---
name: sbc-ef-migration
description: >-
  Runbook for Entity Framework Core data model changes, Fluent API
  configurations, deterministic seeders, and SQL Server migrations in SBC. Use
  whenever adding or modifying entities in SBC.Domain.Entities, updating
  configurations/seeders in SBC.Infrastructure/Database, or changing schemas.
---

# EF Core Schema Changes, Configurations & Migrations in SBC

## 1. Entity & `SbcDbContext` Rules

- **`BaseEntity` inheritance**: Every persistent entity (except `ApplicationUser`, which inherits from `IdentityUser<Guid>`) must inherit from `BaseEntity` (`Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`).
- **Automatic audit fields**: `SbcDbContext.SaveChangesAsync` (`SBC.Infrastructure/Database/SbcDbContext.cs`) automatically sets `CreatedAt`/`CreatedBy` on `Added` and `UpdatedAt`/`UpdatedBy` on `Modified` using `DateTime.Now` and `httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system"`. Do not set or overwrite these manually in services.
- **DbSet registration**: Add `public DbSet<MyEntity> MyEntities { get; set; }` to `SbcDbContext`.
- **Query splitting**: `Program.cs` configures `QuerySplittingBehavior.SplitQuery` globally; avoid redundant per-query configuration.

## 2. `IEntityTypeConfiguration<T>` Conventions

Create `<Entity>Configuration : IEntityTypeConfiguration<Entity>` in `SBC.Infrastructure/Database/Configurations/`. `SbcDbContext.OnModelCreating` discovers configurations automatically via `ApplyConfigurationsFromAssembly`.

Verify every item:
- **Correct SQL schema**:
  - `accounting`: accounting entities (`Accounts`, `AccountingPeriods`, `BulkImports`, `JournalEntries`, `JournalEntryLines`).
  - `identity`: ASP.NET Core Identity tables (configured directly in `SbcDbContext.OnModelCreating`).
  - `logging`: auditing (`TransactionLogs`).
- **Primary keys**:
  - In the `accounting` schema, explicitly configure `builder.HasKey(x => x.Id);` and `builder.Property(x => x.Id).ValueGeneratedNever();` (services assign `Guid.NewGuid()`).
- **Explicit column types & max lengths**:
  - Monetary columns (`Debit`, `Credit`): `HasColumnType("money")`.
  - Dates: `HasColumnType("datetime2")`.
  - Small integers (`Year`, `Month`, `Day`): `HasColumnType("smallint")`.
  - Strings: always set `HasMaxLength(...)` and `HasColumnType("varchar(...)")` or `HasColumnType("nvarchar(...)")` as appropriate.
  - Enums (`AccountType`, `TransactionStatus`): persist as strings via `.HasConversion<string>().HasMaxLength(...)`.
- **Constraints & referential integrity**:
  - Add `HasCheckConstraint(...)` inside `builder.ToTable(t => { ... })` for database-level invariants (valid year/month/day ranges, non-negative amounts, non-empty strings, valid enum values).
  - Always set `OnDelete(...)` explicitly:
    - `DeleteBehavior.Cascade` only for strict parent-child aggregates (`JournalEntry` → `JournalEntryLine`).
    - `DeleteBehavior.Restrict` for catalog or historical references (`Account.ParentAccount`, `JournalEntryLine.Account`, `AccountingPeriod.ClosingJournalEntry`).
- **Performance indexes**:
  - Add `HasIndex(...)` for frequently queried columns, unique business keys (`IsUnique()`), and columns used in paged filtering/sorting.

## 3. Seeder Rules (`SBC.Infrastructure/Database/Seeder/`)

Existing seeders (`RolSeeder`, `UserSeeder`, `AccountSeeder`) run through `HasData` in `OnModelCreating`:
- **Strict determinism**: Every `Id`, `ConcurrencyStamp`, `SecurityStamp`, `PasswordHash`, and timestamp (`CreatedAt`) must be constant or deterministically generated (e.g., `AccountSeeder.GenerateGuid(code)`).
- **Never** use `Guid.NewGuid()`, `DateTime.Now`, or `DateTime.UtcNow` in `HasData`, as they generate spurious diffs on every migration.

## 4. Creating & Reviewing Migrations

1. Verify the solution compiles first:
   ```bash
   dotnet build SBC.sln
   ```
2. Generate the migration specifying both migrations and startup projects:
   ```bash
   dotnet ef migrations add <DescriptiveMigrationName> --project SBC.Infrastructure --startup-project SBC.Api
   ```
   *(Use descriptive PascalCase names like `AddBudgetTable` or `AddJournalEntryIndexes`; avoid generic names like `PendingChanges`.)*
3. **Mandatory migration review** (`SBC.Infrastructure/Migrations/<Timestamp>_<Name>.cs`):
   - Inspect both `Up()` and `Down()` for unintended `DropTable`, `DropColumn`, seeder churn, or FK changes that could cause data loss.
   - If any change is destructive to existing data, get explicit user confirmation before applying it.
4. Re-verify build and unit tests:
   ```bash
   dotnet build SBC.sln
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj
   ```
5. When asked to apply the migration locally:
   ```bash
   dotnet ef database update --project SBC.Infrastructure --startup-project SBC.Api
   ```

## 5. Agent Documentation Maintenance

If the change introduces new tables/schemas, seeders, or structural persistence rules, update `AGENTS.md`, this skill (`sbc-ef-migration`), and their mirrors in `docs/agent-guidelines-en/`.
