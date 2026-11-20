---
name: sbc-add-endpoint
description: >-
  Guía paso a paso para agregar un nuevo endpoint, controlador, servicio o
  recurso completo por capas en la API SBC (DataFlowHub API). Úsala siempre que
  se cree una nueva funcionalidad HTTP, un endpoint CRUD o paginado, o un nuevo
  controlador en SBC.Api.
---

# Flujo Completo para Agregar un Endpoint o Recurso en SBC

Sigue este orden de capas de adentro hacia afuera para mantener consistencia arquitectónica, auditoría, paginación y cobertura de pruebas.

## 1. Dominio y Entidades (`SBC.Domain.Entities` y `SBC.Domain`)

1. **Entidad (si es persistente)**:
   - Crea la clase en `SBC.Domain.Entities/<Area>/<Entidad>.cs` heredando de `BaseEntity` (`SBC.Domain.Entities.Base`).
   - No dupliques propiedades de auditoría (`Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`); ya existen en `BaseEntity`.
2. **Acciones de auditoría**:
   - Agrega las constantes necesarias en `SBC.Domain.Entities/Logging/TransactionActions.cs` (ej. `public const string GetX = "GetX";`, `CreateX`, `UpdateX`, `DeleteX`).
3. **Interfaz de repositorio**:
   - Crea `I<Entidad>Repository` en `SBC.Domain/Repositories/Interfaces/` heredando de `IBaseRepository<Entidad>`.
   - Si el recurso expone un listado GET, agrega `GetPagedAsync` retornando una tupla `(IEnumerable<Entidad> Items, int TotalCount)`.

## 2. Infraestructura (`SBC.Infrastructure`)

1. **Configuración EF Core y Migración**:
   - Si agregaste o modificaste entidades, sigue la skill `sbc-ef-migration` (crea `IEntityTypeConfiguration<T>`, agrega `DbSet<T>` en `SbcDbContext` y genera la migración).
2. **Implementación de repositorio**:
   - Crea `<Entidad>Repository(SbcDbContext context) : BaseRepository<Entidad>(context), I<Entidad>Repository` en `SBC.Infrastructure/Repositories/Implementation/`.
   - En `GetPagedAsync`:
     - Parte de `_dbSet.AsNoTracking()`.
     - Aplica filtros opcionales.
     - Calcula `var totalCount = await query.CountAsync();` **antes** de `Include`/`OrderBy`/`Skip`/`Take`.
     - Aplica ordenamiento determinístico, `.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()`.

## 3. Modelos de Aplicación (`SBC.Application.Models`)

1. Crea los DTOs en `SBC.Application.Models/<Area>/`:
   - Usa clases en `Accounting/`, `Dashboard/` y `Logging/`; usa `record` posicionales en `Auth/`.
2. Para endpoints GET paginados, crea `<Entidad>FilterDto : BaseFilterDto` (`SBC.Application.Models.Common`).

## 4. Servicios de Aplicación (`SBC.Application`)

1. Declara `I<Recurso>Service` en `SBC.Application/Services/Interfaces/` y su implementación en `SBC.Application/Services/Implementation/` usando primary constructor.
2. **Excepciones**:
   - Recurso inexistente → `throw new NotFoundException(nameof(Entidad), id);` (`404`).
   - Regla de negocio violada → `throw new DomainException("Mensaje en español.");` (`400`).
   - Otros estados HTTP (`401`, `412`) → `throw new SbcException(HttpStatusCode.<Codigo>, "Mensaje en español.");`.
3. **Asignación de ID**: Para entidades en el esquema `accounting`, asigna `Id = Guid.NewGuid()` antes de `AddAsync`.
4. **Paginación**: Devuelve `PagedResultDto<TDto>` poblando `Items`, `TotalCount`, `PageNumber` y `PageSize`.
5. **Auditoría sin duplicación**:
   - En **mutaciones** (`Create`, `Update`, `Delete` y acciones de negocio): registra éxito y errores con `ITransactionLogService.LogTransactionAsync(...)` usando `TransactionActions`, `nameof(Entidad)`, `entityId`, `TransactionStatus` y payload serializado **sin contraseñas ni tokens**.
   - En **lecturas GET simples**: delega el registro al controlador vía `ExecuteServiceAsync` y **no** llames a `LogTransactionAsync` dentro del servicio para evitar registros duplicados.

## 5. Registro DI y Controlador (`SBC.Api`)

1. **Inyección de dependencias**:
   - Registra repositorio y servicio en `SBC.Api/Program.cs`:
     ```csharp
     builder.Services.AddScoped<I<Entidad>Repository, <Entidad>Repository>();
     builder.Services.AddScoped<I<Recurso>Service, <Recurso>Service>();
     ```
2. **Controlador**:
   - Crea `<Recursos>Controller` en `SBC.Api/Controllers/` heredando de `SbcControllerBase`, con `[Authorize]`, `[ApiController]` y `[Route("api/[controller]")]`.
   - Documenta los endpoints con comentarios XML (`/// <summary>`).
   - Usa `ExecuteServiceAsync`:
     ```csharp
     // Lectura GET (logging automático desde el controlador)
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

     // Creación POST (el servicio registra el TransactionLog con entityId)
     [HttpPost]
     public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemDto dto)
     {
         return await ExecuteServiceAsync(() => service.CreateAsync(dto), HttpStatusCode.Created);
     }

     // Actualización PUT / Eliminación DELETE (devuelve 204 NoContent)
     [HttpPut("{id:guid}")]
     public async Task<ActionResult<SbcGenericResponse>> Update(Guid id, [FromBody] UpdateItemDto dto)
     {
         return await ExecuteServiceAsync(() => service.UpdateAsync(id, dto));
     }
     ```
   - Si el endpoint devuelve un archivo binario (Excel/PDF), retorna `Task<IActionResult>` con `File(bytes, contentType, fileName)` y asegúrate de que el servicio registre el `TransactionLog`.
3. **Archivo HTTP de pruebas**:
   - Agrega el ejemplo de cada nuevo endpoint en `SBC.Api/SBC.Api.http` con `# @name api/<ruta>`, `{{SBC_HostAddress}}` y `Authorization: Bearer {{auth_token}}`.

## 6. Pruebas, Validación y Actualización de Documentación

1. Crea/actualiza tests unitarios en `SBC.UnitTest/Controllers/` y `SBC.UnitTest/Services/` (consulta la skill `sbc-unit-testing`).
2. Ejecuta compilación y pruebas:
   ```bash
   dotnet build SBC.sln
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj
   ```
3. **Mantenimiento de documentación de agentes**: Si este desarrollo introdujo nuevos patrones arquitectónicos, esquemas, dependencias o convenciones, actualiza `AGENTS.md`, las skills afectadas en `.agents/skills/` y su respaldo espejo en `docs/agent-guidelines-en/`.
