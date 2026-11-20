# Directrices del Proyecto SBC (DataFlowHub API)

> Archivo universal de instrucciones para agentes de código y desarrolladores. Aplica a todo el repositorio `SBC`.

## 1. Resumen del Proyecto y Stack Tecnológico

**SBC (DataFlowHub API)** es una API REST de contabilidad general construida sobre **.NET 10** con una arquitectura por capas inspirada en Clean Architecture (entidades de dominio, contratos/excepciones, modelos DTO, servicios de aplicación, persistencia EF Core y capa Web API).

| Componente | Tecnología / Versión verificada |
| :--- | :--- |
| **Runtime / SDK** | .NET 10 (`net10.0`) |
| **Lenguaje** | C# 14 (`Nullable=enable`, `ImplicitUsings=enable`) |
| **Web API** | ASP.NET Core Web API (`SBC.Api`) |
| **ORM y Base de Datos** | Entity Framework Core 10 + SQL Server (`QuerySplittingBehavior.SplitQuery`) |
| **Autenticación y Usuarios** | ASP.NET Core Identity (`ApplicationUser`, `IdentityRole<Guid>`) + JWT Bearer |
| **Documentación API** | Swashbuckle / Swagger OpenAPI (`10.1.7`) con comentarios XML habilitados (`GenerateDocumentationFile=true`) |
| **Excel** | `ClosedXML` (`0.105.0`) — importación masiva de asientos, plantilla y exportación de reportes |
| **PDF** | `QuestPDF` (`2026.2.4`, `LicenseType.Community`) — exportación de reportes financieros |
| **Pruebas Unitarias** | `xUnit` (`2.9.3`), `Moq` (`4.20.72`), `MockQueryable.Moq` (`7.0.3`), `coverlet.collector` (`8.0.1`) |
| **Contenedor** | Docker (`SBC.Api/Dockerfile`, base Linux `aspnet:10.0` / `sdk:10.0`) |

---

## 2. Arquitectura de la Solución y Límites de Dependencias

La solución `SBC.sln` contiene 7 proyectos. Respeta estrictamente la dirección de referencias existente:

| Proyecto | Responsabilidad | Referencias de proyecto |
| :--- | :--- | :--- |
| `SBC.Domain.Entities` | Entidades (`BaseEntity`, `Accounting`, `Logging`), `ApplicationUser`, enums (`AccountType`, `TransactionStatus`) y `TransactionActions` | Ninguna (solo `Microsoft.Extensions.Identity.Stores`) |
| `SBC.Domain` | Interfaces de repositorio (`IBaseRepository<T>`, repos específicos) y excepciones (`SbcException`, `DomainException`, `NotFoundException`) | `SBC.Domain.Entities` |
| `SBC.Application.Models` | DTOs de entrada/salida, `BaseFilterDto`, `PagedResultDto<T>` y DTOs de filtros | `SBC.Domain.Entities` |
| `SBC.Application` | Interfaces e implementaciones de servicios de negocio, importación Excel y generación de reportes Excel/PDF | `SBC.Application.Models`, `SBC.Domain` |
| `SBC.Infrastructure` | `SbcDbContext`, configuraciones Fluent API (`IEntityTypeConfiguration<T>`), seeders, migraciones EF Core e implementaciones de repositorios | `SBC.Application` |
| `SBC.Api` | Punto de entrada (`Program.cs`), controladores (`SbcControllerBase`), `SbcGenericResponse`, configuración JWT/Swagger/CORS y `SBC.Api.http` | `SBC.Infrastructure` (transitivo hacia Application/Domain) |
| `SBC.UnitTest` | Pruebas unitarias organizadas en `Controllers/` y `Services/` | Los 6 proyectos anteriores |

**Regla de capas**:
- Nunca referencies `SBC.Infrastructure` ni `SBC.Api` desde `SBC.Domain`, `SBC.Domain.Entities`, `SBC.Application` o `SBC.Application.Models`.
- La lógica de negocio pertenece a `SBC.Application` (y validaciones intrínsecas a la entidad en `SBC.Domain.Entities`, como `JournalEntry.ValidateDoubleEntry()`), nunca a los controladores.

---

## 3. Comandos Estándar (CLI)

Ejecuta estos comandos desde la raíz del repositorio (`SBC.sln`):

```bash
# Compilar toda la solución
dotnet build SBC.sln

# Ejecutar todas las pruebas unitarias
dotnet test SBC.UnitTest/SBC.UnitTest.csproj

# Ejecutar pruebas filtradas por clase o namespace
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntryServiceTests"

# Crear una nueva migración de EF Core
dotnet ef migrations add <NombreMigracion> --project SBC.Infrastructure --startup-project SBC.Api

# Aplicar migraciones a la base de datos configurada
dotnet ef database update --project SBC.Infrastructure --startup-project SBC.Api

# Ejecutar la API en modo desarrollo (http://localhost:5164 / https://localhost:7182)
dotnet run --project SBC.Api/SBC.Api.csproj
```

---

## 4. Configuración, Secretos y Particularidades del Entorno

- **Claves requeridas en tiempo de ejecución**:
  - `ConnectionStrings:DefaultConnection`
  - `JwtSettings:SecretKey` *(nota: `appsettings.json` contiene una clave placeholder `"Secret"`, pero `Program.cs` y `AuthService.cs` leen explícitamente **`JwtSettings:SecretKey`**)*
  - `JwtSettings:Issuer`
  - `JwtSettings:Audience`
- **Gestión de secretos**: Usa `dotnet user-secrets` (tanto `SBC.Api` como `SBC.Infrastructure` comparten `UserSecretsId`: `b333b6ad-137c-4d3a-844d-74ab1e6bc669`) o variables de entorno. **Nunca** commitees cadenas de conexión reales, contraseñas ni `SecretKey` en `appsettings.json` o `appsettings.Development.json`.
- **CORS**: Actualmente `Program.cs` define la política `AllowSpecificOrigin` con `http://localhost:5173`.
- **Advertencia de paquetes NuGet**: `SBC.Application.csproj` referencia actualmente `Microsoft.EntityFrameworkCore` versión `10.0.0-preview.2.25115.7` mientras `SBC.Api` y `SBC.Infrastructure` usan `10.0.5`. No actualices paquetes no relacionados sin solicitud explícita; tenlo presente si tocas dependencias de EF Core.

---

## 5. Convenciones de Código e Idioma

- **Estilo C# 14 / .NET 10**:
  - Usa **file-scoped namespaces** (`namespace SBC...;`).
  - Usa **primary constructors** en nuevas clases de controladores, servicios y repositorios (siguiendo el estilo moderno de la base de código).
  - Usa PascalCase para tipos, métodos y propiedades; camelCase para variables locales y parámetros (`_campo` solo cuando se declaran campos explícitos).
  - Respeta la convención actual de DTOs: `record` posicionales en `SBC.Application.Models/Auth/` y clases en `Accounting/`, `Dashboard/`, `Logging/` y `Common/`.
  - Agrega documentación XML (`/// <summary>`) en controladores, DTOs e interfaces públicas nuevas (Swagger incluye los comentarios XML de `SBC.Api`).
- **Idioma en el código**:
  - Identificadores, namespaces, nombres de tipos/métodos y comentarios XML en **inglés**.
  - Mensajes de error/validación orientados al usuario y textos visibles de reportes Excel/PDF o plantillas en **español** (ej. `"Cuenta no encontrada."`, `"El asiento no cumple con el principio de partida doble."`).
- **Inyección de Dependencias**:
  - Registra cada nuevo par interfaz/implementación de repositorio y servicio con `builder.Services.AddScoped<I..., ...>()` en `SBC.Api/Program.cs`.

---

## 6. Estándares de API, Controladores y Excepciones

- **Controlador base y autorización**:
  - Todo controlador nuevo debe heredar de `SbcControllerBase` (`SBC.Api.Controllers.Base`), usar `[ApiController]`, `[Route("api/[controller]")]` y `[Authorize]` (salvo endpoints públicos como los de `AuthController`).
- **Ejecución estandarizada (`ExecuteServiceAsync`)**:
  - Envuelve las llamadas a servicios que devuelven JSON con `ExecuteServiceAsync(...)`:
    - `ExecuteServiceAsync<T>(Func<Task<T>>, HttpStatusCode status = HttpStatusCode.OK, string? logAction = null, string? entityName = null, object? parameters = null)` devuelve `SbcGenericResponse<T>` (o `204 NoContent` si `status == HttpStatusCode.NoContent`).
    - `ExecuteServiceAsync(Func<Task>, ...)` devuelve `204 NoContent` cuando tiene éxito.
  - Endpoints de descarga binaria (Excel/PDF como en `FinancialReportsController` o `BulkImportsController.GetTemplate`) devuelven `IActionResult` mediante `File(bytes, contentType, fileName)`.
- **Jerarquía de excepciones y mapeo HTTP**:
  - Usa `DomainException(message)` para violaciones de reglas de negocio (`400 BadRequest`).
  - Usa `NotFoundException(nameof(Entity), id)` para recursos inexistentes (`404 NotFound`).
  - Usa `SbcException(HttpStatusCode, message)` para otros códigos soportados por `SbcControllerBase.GenerateActionResult`: `OK (200)`, `Created (201)`, `NotFound (404)`, `Unauthorized (401)`, `PreconditionFailed (412 -> ProblemDetails)` y cualquier otro código cae en `BadRequest (400)`.
- **Paginación y filtrado obligatorios en colecciones GET**:
  - Todo endpoint GET de colecciones (asientos, líneas, cargas masivas, logs, usuarios, nuevos listados) **DEBE** soportar paginación y filtrado.
  - Crea un DTO de filtro heredando de `BaseFilterDto` (`PageNumber = 1`, `PageSize = 10`).
  - El repositorio implementa `GetPagedAsync(...)` devolviendo una tupla `(IEnumerable<T> Items, int TotalCount)` usando `AsNoTracking()` y calculando `CountAsync()` antes de `Skip`/`Take`.
  - El servicio devuelve `PagedResultDto<T>`.
- **Cliente HTTP (`SBC.Api/SBC.Api.http`)**:
  - Siempre que agregues o modifiques un controlador o endpoint, actualiza `SBC.Api/SBC.Api.http` con su ejemplo de solicitud usando `{{SBC_HostAddress}}` y `Authorization: Bearer {{auth_token}}`.

---

## 7. Auditoría de Transacciones (`TransactionLog`)

- Toda operación principal del sistema (CRUD, lecturas GET, reportes, cargas masivas, autenticación y gestión de usuarios) **DEBE** registrarse mediante `ITransactionLogService`.
- Usa siempre:
  - Constantes en `TransactionActions` (`SBC.Domain.Entities.Logging`) para `action` (agrega nuevas constantes ahí cuando crees nuevas operaciones).
  - `nameof(Entidad)` para `entityName`.
  - El enum `TransactionStatus` (`Success`, `Failure`, `ValidationError`, `PartialSuccess`) para `status`.
  - Detalles serializados con `JsonSerializer.Serialize(...)` y mensaje de error cuando aplique. **Nunca** serialices contraseñas, hashes ni tokens en `Details`.
- **Regla anti-duplicación (evitar doble log)**:
  - **Mutaciones y operaciones complejas** (crear, actualizar, eliminar, cierre de período, importación masiva, login/registro/refresh, generación de reportes): registra la transacción dentro del **servicio** (donde se dispone de `entityId` y distinción entre `ValidationError`/`Failure`/`PartialSuccess`/`Success`) y llama a `ExecuteServiceAsync` en el controlador **sin** `logAction`.
  - **Lecturas GET simples**: registra automáticamente desde el **controlador** pasando `logAction`, `entityName` y `parameters` a `ExecuteServiceAsync`, sin volver a llamar a `LogTransactionAsync` en el servicio:
    ```csharp
    return await ExecuteServiceAsync(
        () => service.GetByIdAsync(id),
        HttpStatusCode.OK,
        TransactionActions.GetAccounts,
        nameof(Account),
        new { id });
    ```

---

## 8. Persistencia (EF Core) e Invariantes Contables

### Persistencia y Base de Datos
- **Esquemas SQL Server**: `accounting` (`Accounts`, `AccountingPeriods`, `BulkImports`, `JournalEntries`, `JournalEntryLines`), `identity` (tablas ASP.NET Core Identity) y `logging` (`TransactionLogs`).
- **Auditoría automática**: Toda entidad de dominio persistente (salvo `ApplicationUser`) hereda de `BaseEntity`. `SbcDbContext.SaveChangesAsync` asigna automáticamente `CreatedAt`/`CreatedBy` y `UpdatedAt`/`UpdatedBy` (usando `DateTime.Now` y `httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system"`). No asignes esos campos manualmente en los servicios.
- **Claves primarias**: Las entidades del esquema `accounting` usan `ValueGeneratedNever()` y generan su `Id = Guid.NewGuid()` en el servicio.
- **Configuración y migraciones**: Define cada entidad con `IEntityTypeConfiguration<T>` en `SBC.Infrastructure/Database/Configurations/` y crea una migración EF Core cada vez que modifiques entidades, configuraciones o seeders.
- **Seeders determinísticos**: `RolSeeder`, `UserSeeder` y `AccountSeeder` usan `HasData` con GUIDs, fechas y hashes determinísticos. Nunca uses `Guid.NewGuid()` ni `DateTime.Now` en seeders.

### Invariantes Contables Obligatorios
1. **Validación de período abierto**: Antes de crear o actualizar cualquier `JournalEntry` (manual o por importación masiva), valida con `IAccountingPeriodRepository.IsPeriodOpenAsync(date.Year, date.Month)` que exista un período abierto.
2. **Partida doble**: Todo asiento creado, editado, modificado vía líneas, importado o generado por cierre debe cumplir `JournalEntry.ValidateDoubleEntry()` (`Sum(Debit) == Sum(Credit)`).
3. **Inmutabilidad de asientos mayorizados**: Si `JournalEntry.IsPosted == true`, no se puede editar ni eliminar el asiento ni ninguna de sus líneas (`DomainException`).
4. **Naturaleza de saldos por `AccountType`**:
   - Deudora (`Debit - Credit`): `Asset`, `Cost`, `Expense`.
   - Acreedora (`Credit - Debit`): `Liability`, `Equity`, `Revenue`.
5. **Reportes provisionales / en vivo (`includeUnposted`)**: Los reportes financieros (Estado de Resultados, Balance General en JSON, Excel y PDF) deben soportar `bool includeUnposted = false`; cuando es `true`, incluyen asientos no mayorizados (`IsPosted == false`). `DashboardService` consulta siempre con `includeUnposted = true`.

*(Para fórmulas detalladas, formato de códigos, columnas Excel y flujo de cierre de período, consulta la skill `sbc-accounting-operations`.)*

---

## 9. Pruebas y Verificación

- Ante cualquier cambio que afecte lógica de negocio, acceso a datos, firmas de API o inyección de dependencias:
  1. Compila la solución (`dotnet build SBC.sln`).
  2. Ejecuta las pruebas (`dotnet test SBC.UnitTest/SBC.UnitTest.csproj`) y asegúrate de que pasen en su totalidad.
- Agrega o actualiza pruebas unitarias en `SBC.UnitTest/Controllers/` y `SBC.UnitTest/Services/` para toda funcionalidad nueva o corrección de errores.

---

## 10. Control de Versiones (Git)

- **Nunca** hagas `git commit` ni `git push` a menos que el usuario lo solicite explícitamente.
- Cuando se solicite un commit, usa mensajes claros y descriptivos del cambio técnico, sin agregar trailers ni metadatos de co-autoría acoplados a un agente concreto (salvo que el usuario pida uno específicamente).

---

## 11. Mantenimiento Continuo de `AGENTS.md`, Skills y Respaldo en Inglés (Obligatorio)

**Regla viva de documentación**: En **cada desarrollo o solicitud** realizada sobre este repositorio, antes de dar la tarea por terminada, evalúa si tus cambios afectan alguna información o regla descrita en `AGENTS.md` o en las skills de `.agents/skills/`.

Debes actualizar `AGENTS.md` y/o las skills correspondientes **en la misma intervención** siempre que:
- Se agreguen, eliminen o renombren proyectos, dependencias NuGet relevantes, esquemas SQL, entidades o seeders.
- Cambien convenciones de controladores, excepciones, paginación, auditoría (`TransactionLog`), configuración/secretos o CORS.
- Se agreguen o modifiquen reglas contables, formatos de códigos, columnas de carga masiva Excel o formatos de exportación de reportes.
- Cambien patrones de testing, comandos de build/test o surja un nuevo flujo repetitivo que justifique crear una nueva skill.

**Sincronización del respaldo en inglés**: Cada vez que modifiques `AGENTS.md` o cualquier archivo en `.agents/skills/<nombre>/SKILL.md`, actualiza también su copia espejo equivalente en `docs/agent-guidelines-en/` (`AGENTS.en.md` o `skills/<nombre>.en.md`) para mantener ambos idiomas 100% alineados.

---

## 12. Skills del Proyecto (`.agents/skills/`)

Consulta la skill especializada correspondiente cuando la tarea involucre alguno de estos flujos:

| Skill | Archivo | Cuándo usarla |
| :--- | :--- | :--- |
| `sbc-add-endpoint` | [.agents/skills/sbc-add-endpoint/SKILL.md](.agents/skills/sbc-add-endpoint/SKILL.md) | Crear o ampliar un recurso, controlador, servicio, repositorio o endpoint completo en la API |
| `sbc-ef-migration` | [.agents/skills/sbc-ef-migration/SKILL.md](.agents/skills/sbc-ef-migration/SKILL.md) | Modificar entidades, configuraciones Fluent API, seeders o generar migraciones EF Core |
| `sbc-accounting-operations` | [.agents/skills/sbc-accounting-operations/SKILL.md](.agents/skills/sbc-accounting-operations/SKILL.md) | Trabajar con asientos, líneas, cuentas, cierres de período, carga masiva Excel o reportes JSON/Excel/PDF |
| `sbc-unit-testing` | [.agents/skills/sbc-unit-testing/SKILL.md](.agents/skills/sbc-unit-testing/SKILL.md) | Escribir, actualizar o depurar pruebas unitarias en `SBC.UnitTest` |
