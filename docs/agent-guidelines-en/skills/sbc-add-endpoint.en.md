---
name: sbc-add-endpoint
description: >-
  Step-by-step vertical-slice guide for adding a new endpoint, controller,
  service, or full resource in the SBC (DataFlowHub API) codebase. Use whenever
  creating new HTTP functionality, CRUD or paged endpoints, or a new controller
  in SBC.Api.
---

# End-to-End Playbook for Adding an Endpoint or Resource in SBC

Follow this inside-out layer order to preserve architectural boundaries, auditing, pagination, and unit test coverage.

## 1. Domain & Entities (`SBC.Domain.Entities` and `SBC.Domain`)

1. **Entity (if persistent)**:
   - Create the class in `SBC.Domain.Entities/<Area>/<Entity>.cs` inheriting from `BaseEntity` (`SBC.Domain.Entities.Base`).
   - Do not redeclare audit properties (`Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`); they come from `BaseEntity`.
2. **Audit action constants**:
   - Add constants to `SBC.Domain.Entities/Logging/TransactionActions.cs` (e.g., `public const string GetX = "GetX";`, `CreateX`, `UpdateX`, `DeleteX`).
3. **Repository interface**:
   - Create `I<Entity>Repository` in `SBC.Domain/Repositories/Interfaces/` extending `IBaseRepository<Entity>`.
   - If the resource exposes a collection GET endpoint, add `GetPagedAsync` returning a tuple `(IEnumerable<Entity> Items, int TotalCount)`.

## 2. Infrastructure (`SBC.Infrastructure`)

1. **EF Core configuration & migration**:
   - If entities were added or modified, follow the `sbc-ef-migration` skill (`IEntityTypeConfiguration<T>`, `DbSet<T>` in `SbcDbContext`, and EF Core migration).
2. **Repository implementation**:
   - Create `<Entity>Repository(SbcDbContext context) : BaseRepository<Entity>(context), I<Entity>Repository` in `SBC.Infrastructure/Repositories/Implementation/`.
   - In `GetPagedAsync`:
     - Start from `_dbSet.AsNoTracking()`.
     - Apply optional filters.
     - Compute `var totalCount = await query.CountAsync();` **before** `Include`/`OrderBy`/`Skip`/`Take`.
     - Apply deterministic ordering, `.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()`.

## 3. Application Models (`SBC.Application.Models`)

1. Add DTOs in `SBC.Application.Models/<Area>/`:
   - Use classes in `Accounting/`, `Dashboard/`, and `Logging/`; use positional `record` types in `Auth/`.
2. For paged GET endpoints, create `<Entity>FilterDto : BaseFilterDto` (`SBC.Application.Models.Common`).

## 4. Application Services (`SBC.Application`)

1. Declare `I<Resource>Service` in `SBC.Application/Services/Interfaces/` and implement it in `SBC.Application/Services/Implementation/` using a primary constructor.
2. **Exceptions**:
   - Missing resource → `throw new NotFoundException(nameof(Entity), id);` (`404`).
   - Business rule failure → `throw new DomainException("Mensaje en español.");` (`400`).
   - Other HTTP statuses (`401`, `412`) → `throw new SbcException(HttpStatusCode.<Code>, "Mensaje en español.");`.
3. **Primary key assignment**: For entities in the `accounting` schema, assign `Id = Guid.NewGuid()` before `AddAsync`.
4. **Pagination**: Return `PagedResultDto<TDto>` populating `Items`, `TotalCount`, `PageNumber`, and `PageSize`.
5. **Transaction auditing without duplication**:
   - **Mutations** (`Create`, `Update`, `Delete`, and domain workflows): log success and error outcomes via `ITransactionLogService.LogTransactionAsync(...)` with `TransactionActions`, `nameof(Entity)`, `entityId`, `TransactionStatus`, and sanitized serialized details (**never** passwords or tokens).
   - **Simple GET reads**: delegate logging to the controller via `ExecuteServiceAsync` and do **not** call `LogTransactionAsync` inside the service so logs are not duplicated.

## 5. DI Registration & Controller (`SBC.Api`)

1. **Dependency injection**:
   - Register repository and service in `SBC.Api/Program.cs`:
     ```csharp
     builder.Services.AddScoped<I<Entity>Repository, <Entity>Repository>();
     builder.Services.AddScoped<I<Resource>Service, <Resource>Service>();
     ```
2. **Controller**:
   - Create `<Resources>Controller` in `SBC.Api/Controllers/` inheriting from `SbcControllerBase`, with `[Authorize]`, `[ApiController]`, and `[Route("api/[controller]")]`.
   - Document endpoints with XML comments (`/// <summary>`).
   - Wrap calls using `ExecuteServiceAsync`:
     ```csharp
     // GET read (automatic logging from controller)
     [HttpGet]
     public async Task<ActionResult<PagedResultDto<ItemDto>>> GetPaged([FromQuery] ItemFilterDto filter)
     {
         return await ExecuteServiceAsync(
             () => service.GetPagedAsync(filter),
             HttpStatusCode.OK,
             TransactionActions.GetItems,
             nameof(Item),
             filter);
     }

     // POST create (service logs TransactionLog with entityId)
     [HttpPost]
     public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemDto dto)
     {
         return await ExecuteServiceAsync(() => service.CreateAsync(dto), HttpStatusCode.Created);
     }

     // PUT update / DELETE (returns 204 NoContent)
     [HttpPut("{id:guid}")]
     public async Task<ActionResult<SbcGenericResponse>> Update(Guid id, [FromBody] UpdateItemDto dto)
     {
         return await ExecuteServiceAsync(() => service.UpdateAsync(id, dto));
     }
     ```
   - For binary file endpoints (Excel/PDF), return `Task<IActionResult>` using `File(bytes, contentType, fileName)` and ensure the service records the `TransactionLog`.
3. **HTTP client examples**:
   - Add request examples for every new endpoint to `SBC.Api/SBC.Api.http` using `# @name api/<route>`, `{{SBC_HostAddress}}`, and `Authorization: Bearer {{auth_token}}`.

## 6. Tests, Verification & Agent Docs Maintenance

1. Add or update unit tests in `SBC.UnitTest/Controllers/` and `SBC.UnitTest/Services/` (see `sbc-unit-testing`).
2. Build and test the solution:
   ```bash
   dotnet build SBC.sln
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj
   ```
3. **Agent documentation maintenance**: If this work introduced new architectural patterns, schemas, dependencies, or conventions, update `AGENTS.md`, affected skills in `.agents/skills/`, and their mirrors in `docs/agent-guidelines-en/`.
