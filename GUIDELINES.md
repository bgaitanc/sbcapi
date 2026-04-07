# Project Guidelines

## Project Overview
**SBC (DataFlowHub API)** is a .NET 10 accounting system API. It follows a clean architecture-inspired structure with separated layers for domain, infrastructure, application logic, and the web API.

### Project Structure
- **SBC.Api**: The entry point of the application (ASP.NET Core Web API). Handles HTTP requests, authentication (JWT), and Swagger documentation.
- **SBC.Application**: Contains the application services and business logic implementations.
- **SBC.Application.Models**: DTOs (Data Transfer Objects) and models used by the application layer.
- **SBC.Domain**: Contains core domain interfaces (repositories), exceptions, and domain-level logic.
- **SBC.Domain.Entities**: Domain entities, enums, and Identity-related classes (e.g., `ApplicationUser`).
- **SBC.Infrastructure**: Implementation of data access using Entity Framework Core with SQL Server, including migrations and database configurations.
- **SBC.UnitTest**: Project containing unit tests for various layers.

## Tech Stack
- **Framework**: .NET 10
- **Language**: C# 15
- **ORM**: Entity Framework Core (SQL Server)
- **Authentication**: JWT Bearer + ASP.NET Core Identity
- **API Documentation**: Swagger/OpenAPI

## Development Guidelines

### 1. Code Style and Standards
- Follow standard C# 15 and .NET naming conventions (PascalCase for classes/methods, camelCase for local variables).
- Maintain consistency with the existing codebase patterns.
- Ensure that any new domain entities or database changes are followed by creating an EF Core migration in `SBC.Infrastructure`.

### 2. Testing
- Junie should run tests to check the correctness of the proposed solution when changes affect business logic or data access.
- Tests can be run using the `run_test` tool, targeting `SBC.UnitTest.csproj` or specific namespaces.
- Before submitting, ensure that all relevant tests in `SBC.UnitTest` pass.

### 3. Building the Project
- Junie should verify that the project builds after making changes, especially if they involve API signatures or dependency injection updates in `Program.cs`.

### 4. Database Migrations
- When modifying entities in `SBC.Domain.Entities`, remember to add a migration in the `SBC.Infrastructure` project:
  `dotnet ef migrations add <MigrationName> --project SBC.Infrastructure --startup-project SBC.Api`

### 5. Git Commits
- Only commit when explicitly requested.
- Use descriptive commit messages and always include Junie as a co-author: `--trailer "Co-authored-by: Junie <junie@jetbrains.com>"`

### 6. API Client Testing (SBC.Api.http)
- Whenever a new controller or endpoint is added, update the `SBC.Api\SBC.Api.http` file with its corresponding request example.

### 7. Transaction Logging (TransactionLog)
- All main system operations (CRUD, reports, bulk imports, authentication, etc.) MUST be logged using `ITransactionLogService`.
- **Endpoint Call Logging**: All endpoint calls, including read operations (GET), MUST be logged. This includes recording the parameters used in the call.
- Use the static class `TransactionActions` (in `SBC.Domain.Entities.Logging`) for the `action` parameter.
- Use `nameof(Entity)` for the `entityName` parameter.
- Use the `TransactionStatus` enum (in `SBC.Domain.Entities.Enums`) for the `status` parameter.
- Log relevant transaction details (serialized parameters) and error messages when applicable to facilitate auditing.

### 8. API Controllers and Service Execution
- All new controllers must inherit from `SbcControllerBase` (located in `SBC.Api.Controllers.Base`).
- Use the `ExecuteServiceAsync` method provided by `SbcControllerBase` to wrap service calls. This ensures a standardized response format using `ResultDto` or `PagedResultDto`.
- **Automatic Logging**: When calling `ExecuteServiceAsync` from a controller, provide the optional parameters `logAction`, `entityName`, and `parameters` to automatically log the endpoint call and its arguments.
  ```csharp
  return await ExecuteServiceAsync(() => _service.GetData(id), HttpStatusCode.OK, TransactionActions.GetData, nameof(Entity), new { id });
  ```

### 9. Pagination and Filtering
- All GET endpoints for collections (Journal Entries, Lines, Bulk Imports, etc.) MUST support pagination and filtering.
- Use `BaseFilterDto` as a base for filter DTOs.
- Repositories should implement a `GetPagedAsync` method that returns a tuple `(IEnumerable<T> Items, int TotalCount)`.
- Services should return `PagedResultDto<T>`.

### 10. Accounting Period Validation
- Before creating or updating any `JournalEntry` (manually or via bulk import), always verify that the accounting period for the entry date exists and is open using `IAccountingPeriodRepository.IsPeriodOpenAsync`.

### 11. Live/Provisional Financial Reports
- Financial reports (Income Statement, Balance Sheet) and Dashboard data should support an `includeUnposted` flag.
- When `includeUnposted` is `true`, the system MUST include non-posted (`IsPosted = false`) journal entries in calculations to provide a real-time view of the accounting state.
