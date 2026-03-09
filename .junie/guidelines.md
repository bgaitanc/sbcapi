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
