# SBC Project Guidelines (DataFlowHub API)

> Universal instructions file for coding agents and developers. Applies to the entire `SBC` repository.

## 1. Project Overview & Tech Stack

**SBC (DataFlowHub API)** is a general accounting REST API built on **.NET 10** following a layered architecture inspired by Clean Architecture (domain entities, contracts/exceptions, DTO models, application services, EF Core persistence, and Web API layer).

| Component | Verified Technology / Version |
| :--- | :--- |
| **Runtime / SDK** | .NET 10 (`net10.0`) |
| **Language** | C# 14 (`Nullable=enable`, `ImplicitUsings=enable`) |
| **Web API** | ASP.NET Core Web API (`SBC.Api`) |
| **ORM & Database** | Entity Framework Core 10 + SQL Server (`QuerySplittingBehavior.SplitQuery`) |
| **Auth & Identity** | ASP.NET Core Identity (`ApplicationUser`, `IdentityRole<Guid>`) + JWT Bearer |
| **API Documentation** | Swashbuckle / Swagger OpenAPI (`10.1.7`) with XML comments enabled (`GenerateDocumentationFile=true`) |
| **Excel** | `ClosedXML` (`0.105.0`) — bulk journal entry import, template generation, and report exports |
| **PDF** | `QuestPDF` (`2026.2.4`, `LicenseType.Community`) — financial report exports |
| **Unit Testing** | `xUnit` (`2.9.3`), `Moq` (`4.20.72`), `MockQueryable.Moq` (`7.0.3`), `coverlet.collector` (`8.0.1`) |
| **Container** | Docker (`SBC.Api/Dockerfile`, Linux `aspnet:10.0` / `sdk:10.0`) |

---

## 2. Solution Architecture & Dependency Boundaries

`SBC.sln` contains 7 projects. Strictly preserve the existing reference direction:

| Project | Responsibility | Project References |
| :--- | :--- | :--- |
| `SBC.Domain.Entities` | Entities (`BaseEntity`, `Accounting`, `Logging`), `ApplicationUser`, enums (`AccountType`, `TransactionStatus`), and `TransactionActions` | None (only `Microsoft.Extensions.Identity.Stores`) |
| `SBC.Domain` | Repository interfaces (`IBaseRepository<T>`, specific repositories) and exceptions (`SbcException`, `DomainException`, `NotFoundException`) | `SBC.Domain.Entities` |
| `SBC.Application.Models` | Input/output DTOs, `BaseFilterDto`, `PagedResultDto<T>`, and filter DTOs | `SBC.Domain.Entities` |
| `SBC.Application` | Business service interfaces & implementations, Excel import/templates, and Excel/PDF report generation | `SBC.Application.Models`, `SBC.Domain` |
| `SBC.Infrastructure` | `SbcDbContext`, Fluent API configurations (`IEntityTypeConfiguration<T>`), seeders, EF Core migrations, and repository implementations | `SBC.Application` |
| `SBC.Api` | Entry point (`Program.cs`), controllers (`SbcControllerBase`), `SbcGenericResponse`, JWT/Swagger/CORS setup, and `SBC.Api.http` | `SBC.Infrastructure` (transitively brings Application/Domain) |
| `SBC.UnitTest` | Unit tests organized in `Controllers/` and `Services/` | All 6 projects above |

**Layering rules**:
- Never reference `SBC.Infrastructure` or `SBC.Api` from `SBC.Domain`, `SBC.Domain.Entities`, `SBC.Application`, or `SBC.Application.Models`.
- Business logic belongs in `SBC.Application` (with entity-intrinsic invariants in `SBC.Domain.Entities`, such as `JournalEntry.ValidateDoubleEntry()`), never in controllers.

---

## 3. Standard CLI Commands

Run these commands from the repository root (`SBC.sln`):

```bash
# Build the entire solution
dotnet build SBC.sln

# Run all unit tests
dotnet test SBC.UnitTest/SBC.UnitTest.csproj

# Run filtered unit tests by class or namespace
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntryServiceTests"

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project SBC.Infrastructure --startup-project SBC.Api

# Apply migrations to the configured database
dotnet ef database update --project SBC.Infrastructure --startup-project SBC.Api

# Run the API locally in development (http://localhost:5164 / https://localhost:7182)
dotnet run --project SBC.Api/SBC.Api.csproj
```

---

## 4. Configuration, Secrets & Environment Notes

- **Required runtime settings**:
  - `ConnectionStrings:DefaultConnection`
  - `JwtSettings:SecretKey` *(note: `appsettings.json` contains a placeholder `"Secret"` key, but `Program.cs` and `AuthService.cs` explicitly read **`JwtSettings:SecretKey`**)*
  - `JwtSettings:Issuer`
  - `JwtSettings:Audience`
- **Secret management**: Use `dotnet user-secrets` (both `SBC.Api` and `SBC.Infrastructure` share `UserSecretsId`: `b333b6ad-137c-4d3a-844d-74ab1e6bc669`) or environment variables. **Never** commit real connection strings, passwords, or `SecretKey` values to `appsettings.json` or `appsettings.Development.json`.
- **CORS**: `Program.cs` currently defines the `AllowSpecificOrigin` policy allowing `http://localhost:5173`.
- **NuGet version note**: `SBC.Application.csproj` currently references `Microsoft.EntityFrameworkCore` `10.0.0-preview.2.25115.7` while `SBC.Api` and `SBC.Infrastructure` use `10.0.5`. Do not upgrade unrelated packages unless explicitly requested; keep this in mind when modifying EF Core dependencies.

---

## 5. Coding & Language Conventions

- **C# 14 / .NET 10 style**:
  - Use **file-scoped namespaces** (`namespace SBC...;`).
  - Use **primary constructors** in new controllers, services, and repositories (matching the modern codebase style).
  - Use PascalCase for types, methods, and properties; camelCase for local variables and parameters (`_field` only for explicit backing fields).
  - Follow the existing DTO conventions: positional `record` types in `SBC.Application.Models/Auth/` and classes in `Accounting/`, `Dashboard/`, `Logging/`, and `Common/`.
  - Add XML doc comments (`/// <summary>`) on new public controllers, DTOs, and interfaces (Swagger loads XML documentation from `SBC.Api`).
- **Natural language conventions**:
  - Code identifiers, namespaces, type/method names, and XML comments are in **English**.
  - User-facing validation/error messages and visible Excel/PDF report or template text are in **Spanish** (e.g., `"Cuenta no encontrada."`, `"El asiento no cumple con el principio de partida doble."`).
- **Dependency Injection**:
  - Register every new repository and service interface/implementation pair with `builder.Services.AddScoped<I..., ...>()` in `SBC.Api/Program.cs`.

---

## 6. API, Controller & Exception Standards

- **Base controller & authorization**:
  - Every new controller must inherit from `SbcControllerBase` (`SBC.Api.Controllers.Base`), apply `[ApiController]`, `[Route("api/[controller]")]`, and `[Authorize]` (except intentionally public endpoints like `AuthController`).
- **Standardized execution (`ExecuteServiceAsync`)**:
  - Wrap JSON-returning service calls with `ExecuteServiceAsync(...)`:
    - `ExecuteServiceAsync<T>(Func<Task<T>>, HttpStatusCode status = HttpStatusCode.OK, string? logAction = null, string? entityName = null, object? parameters = null)` wraps results in `SbcGenericResponse<T>` (or returns `204 NoContent` when `status == HttpStatusCode.NoContent`).
    - `ExecuteServiceAsync(Func<Task>, ...)` returns `204 NoContent` on success.
  - Binary download endpoints (Excel/PDF such as `FinancialReportsController` or `BulkImportsController.GetTemplate`) return `IActionResult` via `File(bytes, contentType, fileName)`.
- **Exception hierarchy & HTTP mapping**:
  - Throw `DomainException(message)` for business rule violations (`400 BadRequest`).
  - Throw `NotFoundException(nameof(Entity), id)` for missing resources (`404 NotFound`).
  - Throw `SbcException(HttpStatusCode, message)` for other HTTP codes handled by `SbcControllerBase.GenerateActionResult`: `OK (200)`, `Created (201)`, `NotFound (404)`, `Unauthorized (401)`, `PreconditionFailed (412 -> ProblemDetails)`, with all other codes falling back to `BadRequest (400)`.
- **Mandatory pagination & filtering on collection GET endpoints**:
  - All collection GET endpoints (journal entries, lines, bulk imports, logs, users, new listings) **MUST** support pagination and filtering.
  - Inherit filter DTOs from `BaseFilterDto` (`PageNumber = 1`, `PageSize = 10`).
  - Repositories implement `GetPagedAsync(...)` returning a tuple `(IEnumerable<T> Items, int TotalCount)` using `AsNoTracking()` and computing `CountAsync()` before `Skip`/`Take`.
  - Services return `PagedResultDto<T>`.
- **HTTP client file (`SBC.Api/SBC.Api.http`)**:
  - Whenever a controller or endpoint is added or changed, update `SBC.Api/SBC.Api.http` with a corresponding request example using `{{SBC_HostAddress}}` and `Authorization: Bearer {{auth_token}}`.

---

## 7. Transaction Auditing (`TransactionLog`)

- All major system operations (CRUD, GET reads, reports, bulk imports, authentication, and user management) **MUST** be audited using `ITransactionLogService`.
- Always use:
  - Constants in `TransactionActions` (`SBC.Domain.Entities.Logging`) for `action` (add new constants there for new operations).
  - `nameof(Entity)` for `entityName`.
  - The `TransactionStatus` enum (`Success`, `Failure`, `ValidationError`, `PartialSuccess`) for `status`.
  - Serialized parameters via `JsonSerializer.Serialize(...)` and error messages when applicable. **Never** serialize passwords, password hashes, or tokens into `Details`.
- **No-duplication rule (avoid double logging)**:
  - **Mutations & complex domain operations** (create, update, delete, period close, bulk import, login/register/refresh, report generation): log inside the **service** (where `entityId` and granular `ValidationError`/`Failure`/`PartialSuccess`/`Success` status are available), and call `ExecuteServiceAsync` in the controller **without** `logAction`.
  - **Simple GET reads**: log automatically from the **controller** by passing `logAction`, `entityName`, and `parameters` to `ExecuteServiceAsync`, without calling `LogTransactionAsync` again in the service:
    ```csharp
    return await ExecuteServiceAsync(
        () => service.GetByIdAsync(id),
        HttpStatusCode.OK,
        TransactionActions.GetAccounts,
        nameof(Account),
        new { id });
    ```

---

## 8. Persistence (EF Core) & Accounting Invariants

### Persistence & Database
- **SQL Server schemas**: `accounting` (`Accounts`, `AccountingPeriods`, `BulkImports`, `JournalEntries`, `JournalEntryLines`), `identity` (ASP.NET Core Identity tables), and `logging` (`TransactionLogs`).
- **Automatic audit fields**: All persistent domain entities (except `ApplicationUser`) inherit from `BaseEntity`. `SbcDbContext.SaveChangesAsync` automatically populates `CreatedAt`/`CreatedBy` and `UpdatedAt`/`UpdatedBy` (using `DateTime.Now` and `httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system"`). Do not set these manually in services.
- **Primary keys**: Entities in the `accounting` schema configure `ValueGeneratedNever()` and assign `Id = Guid.NewGuid()` in services.
- **Configuration & migrations**: Define Fluent API rules with `IEntityTypeConfiguration<T>` in `SBC.Infrastructure/Database/Configurations/` and generate an EF Core migration for every entity, configuration, or seeder change.
- **Deterministic seeders**: `RolSeeder`, `UserSeeder`, and `AccountSeeder` use `HasData` with deterministic GUIDs, fixed dates, and static hashes. Never use `Guid.NewGuid()` or `DateTime.Now` in seeders.

### Mandatory Accounting Invariants
1. **Open accounting period check**: Before creating or updating any `JournalEntry` (manual or bulk import), verify an open period exists via `IAccountingPeriodRepository.IsPeriodOpenAsync(date.Year, date.Month)`.
2. **Double-entry bookkeeping**: Every created, updated, line-modified, bulk-imported, or closing journal entry must satisfy `JournalEntry.ValidateDoubleEntry()` (`Sum(Debit) == Sum(Credit)`).
3. **Immutability of posted entries**: When `JournalEntry.IsPosted == true`, neither the entry nor any of its lines may be edited or deleted (`DomainException`).
4. **Normal balances by `AccountType`**:
   - Debit-normal (`Debit - Credit`): `Asset`, `Cost`, `Expense`.
   - Credit-normal (`Credit - Debit`): `Liability`, `Equity`, `Revenue`.
5. **Provisional / live reports (`includeUnposted`)**: Financial reports (Income Statement, Balance Sheet across JSON, Excel, and PDF) must support `bool includeUnposted = false`; when `true`, include non-posted (`IsPosted == false`) entries. `DashboardService` always queries with `includeUnposted = true`.

*(For detailed formulas, code formats, Excel layouts, and period closing steps, see the `sbc-accounting-operations` skill.)*

---

## 9. Testing & Verification

- Whenever changes affect business logic, data access, API signatures, or dependency injection:
  1. Build the solution (`dotnet build SBC.sln`).
  2. Run unit tests (`dotnet test SBC.UnitTest/SBC.UnitTest.csproj`) and ensure all tests pass.
- Add or update unit tests in `SBC.UnitTest/Controllers/` and `SBC.UnitTest/Services/` for every new feature or bug fix.

---

## 10. Version Control (Git)

- **Never** run `git commit` or `git push` unless explicitly requested by the user.
- When committing upon request, use clear, descriptive commit messages without agent-specific trailers or co-author tags (unless the user explicitly asks for one).

---

## 11. Continuous Maintenance of `AGENTS.md`, Skills & English Mirror (Mandatory)

**Living documentation rule**: For **every development task or request** in this repository, before considering the task complete, evaluate whether your changes affect any directive, architecture detail, or workflow documented in `AGENTS.md` or `.agents/skills/`.

You MUST update `AGENTS.md` and/or the affected skills **in the same change** whenever:
- Projects, notable NuGet dependencies, SQL schemas, entities, or seeders are added, removed, or renamed.
- Controller, exception, pagination, transaction logging, configuration/secret, or CORS conventions change.
- Accounting business rules, code formats, bulk Excel column layouts, or report export formats change.
- Testing patterns or CLI workflows evolve, or a new repeatable workflow warrants creating a new skill.

**English mirror synchronization**: Whenever `AGENTS.md` or any `.agents/skills/<name>/SKILL.md` is updated, update its corresponding mirror file in `docs/agent-guidelines-en/` (`AGENTS.en.md` or `skills/<name>.en.md`) so both versions stay 100% aligned.

---

## 12. Project Skills Index (`.agents/skills/`)

Consult the relevant specialized skill when a task involves any of these workflows:

| Skill | File | When to Use |
| :--- | :--- | :--- |
| `sbc-add-endpoint` | [.agents/skills/sbc-add-endpoint/SKILL.md](.agents/skills/sbc-add-endpoint/SKILL.md) | Adding or extending a full API resource, controller, service, repository, or endpoint |
| `sbc-ef-migration` | [.agents/skills/sbc-ef-migration/SKILL.md](.agents/skills/sbc-ef-migration/SKILL.md) | Modifying entities, Fluent API configurations, seeders, or generating EF Core migrations |
| `sbc-accounting-operations` | [.agents/skills/sbc-accounting-operations/SKILL.md](.agents/skills/sbc-accounting-operations/SKILL.md) | Working on journal entries, lines, accounts, period closing, Excel bulk import, or JSON/Excel/PDF financial reports |
| `sbc-unit-testing` | [.agents/skills/sbc-unit-testing/SKILL.md](.agents/skills/sbc-unit-testing/SKILL.md) | Writing, updating, or debugging unit tests in `SBC.UnitTest` |
