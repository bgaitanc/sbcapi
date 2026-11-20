# Registro de Deuda Técnica — SBC (DataFlowHub API)

> [!WARNING]
> **Este archivo NO es base de conocimiento ni documentación normativa.**
> Es un **backlog vivo de deuda técnica y mejoras pendientes** identificadas en el repositorio `SBC` para ser atendidas posteriormente.
> Agentes de código y desarrolladores **NO deben** tomar los comportamientos o limitaciones aquí descritos como convenciones válidas ni como referencia para implementar nuevas funcionalidades.

---

## Cómo Usar y Mantener Este Documento

- **Estados permitidos**: `Pendiente` · `En progreso` · `Resuelto`
- **Niveles de prioridad**:
  - `Alta`: vulnerabilidades de seguridad, riesgo de corrupción/inconsistencia de datos o errores graves de autorización/manejo de excepciones.
  - `Media`: dependencias inconsistentes, bugs de auditoría, cuellos de botella de rendimiento, validaciones faltantes o brechas de cobertura de pruebas.
  - `Baja`: advertencias del compilador, mejoras de experiencia de desarrollo (DevEx/Docker) e higiene de código.
- **Cuándo actualizarlo**:
  - Al resolver un ítem: cambiar su estado a `Resuelto` (o retirarlo si ya no aporta historial útil) y limpiar cualquier advertencia temporal asociada en [AGENTS.md](../AGENTS.md) y su espejo en inglés [AGENTS.en.md](agent-guidelines-en/AGENTS.en.md).
  - Al detectar deuda técnica fuera del alcance de una tarea: agregar un nuevo ítem secuencial (`TD-XX`) con ubicación exacta, impacto y criterio de cierre.

---

## Resumen Ejecutivo

| ID | Prioridad | Categoría | Estado | Título |
| :--- | :---: | :--- | :---: | :--- |
| `TD-01` | Alta | Seguridad / NuGet | Pendiente | Vulnerabilidad alta (`NU1903`) en paquete transitivo `Microsoft.OpenApi 2.4.1` |
| `TD-02` | Alta | Seguridad / Auth | Pendiente | Autorización por rol `Admin` desactivada en `UsersController` |
| `TD-03` | Alta | Integridad de Datos | Pendiente | Ausencia de transacciones explícitas (Unit of Work) en operaciones multi-escritura |
| `TD-04` | Alta | Configuración / Consistencia | Pendiente | Discrepancia `JwtSettings:Secret` vs `SecretKey`, CORS hardcodeado y campos de auditoría sin asignar |
| `TD-05` | Alta | Manejo de Errores | Pendiente | Excepciones no controladas devueltas como `400 BadRequest` exponiendo `e.Message` y uso de `new Exception(...)` |
| `TD-06` | Media | Dependencias / Arquitectura | Pendiente | Referencia preview obsoleta de EF Core (`10.0.0-preview.2.25115.7`, `NU1603`) en `SBC.Application.csproj` |
| `TD-07` | Media | Auditoría (`TransactionLog`) | Pendiente | Doble logging en endpoints paginados y brechas de auditoría en usuarios y reportes binarios |
| `TD-08` | Media | Rendimiento | Pendiente | Consultas N+1 en usuarios/carga masiva y agregación histórica en memoria en reportes/dashboard |
| `TD-09` | Media | Fechas / Zonas Horarias | Pendiente | Uso mezclado de `DateTime.Now`, `DateTime.UtcNow` y `DateTimeKind.Unspecified` |
| `TD-10` | Media | Validación / Dominio | Pendiente | Validaciones faltantes en paginación, cierre de período, exclusividad débito/crédito y `BaseEntity` |
| `TD-11` | Media | Pruebas | Pendiente | Brechas de cobertura unitaria (5 controladores, 2 servicios) y ausencia de pruebas de integración |
| `TD-12` | Baja | Calidad / Compilador | Pendiente | 19 advertencias activas del compilador C# (`CS8618`, `CS9107`, `CS8619`, `CS8600`) |
| `TD-13` | Baja | DevEx / Docker | Pendiente | `Dockerfile` incompleto para restore multi-proyecto y endpoints faltantes en `SBC.Api.http` |
| `TD-14` | Baja | Higiene de Código | Pendiente | `using` sin usar, método `async` sin `await`, constructores mixtos y errores tipográficos en `AccountSeeder` |

---

## Prioridad Alta

### `TD-01` — Vulnerabilidad de severidad alta en `Microsoft.OpenApi 2.4.1` (`NU1903`)
- **Estado**: `Pendiente`
- **Categoría**: Seguridad / NuGet
- **Ubicación**: `SBC.Api/SBC.Api.csproj` (`Swashbuckle.AspNetCore 10.1.7`), `SBC.UnitTest/SBC.UnitTest.csproj`
- **Problema**: `dotnet build` reporta advertencia `NU1903` indicando que el paquete transitivo `Microsoft.OpenApi 2.4.1` tiene una vulnerabilidad conocida de severidad alta (`GHSA-v5pm-xwqc-g5wc`).
- **Acción recomendada**: Actualizar `Swashbuckle.AspNetCore` a una versión que consuma una versión parcheada de `Microsoft.OpenApi`, o fijar explícitamente la versión parcheada de `Microsoft.OpenApi` si es compatible.
- **Criterio de cierre**: `dotnet build SBC.sln` sin advertencias `NU1903` y Swagger UI (`/swagger`) operativo.

### `TD-02` — Autorización basada en roles desactivada en `UsersController`
- **Estado**: `Pendiente`
- **Categoría**: Seguridad / Autenticación y Autorización
- **Ubicación**: `SBC.Api/Controllers/UsersController.cs` (líneas 16–17)
- **Problema**: El atributo `[Authorize(Roles = "Admin")]` está comentado y reemplazado por `[Authorize]`. Como resultado, cualquier usuario autenticado (incluido el rol `Guest` asignado por defecto en el registro público) puede listar, crear y editar usuarios, cambiar roles y modificar contraseñas de terceros.
- **Acción recomendada**: Reactivar `[Authorize(Roles = "Admin")]` para administración de usuarios/roles y, si un usuario debe poder cambiar su propia contraseña o consultar su perfil, exponer endpoints dedicados que obtengan el `userId` exclusivamente desde los claims del token JWT.
- **Criterio de cierre**: Usuarios con rol `Guest` reciben `403 Forbidden` en operaciones administrativas de `UsersController`, verificado mediante pruebas.

### `TD-03` — Operaciones multi-escritura sin transacción explícita (Unit of Work)
- **Estado**: `Pendiente`
- **Categoría**: Integridad de Datos
- **Ubicación**:
  - `SBC.Application/Services/Implementation/AccountingPeriodService.cs` (`ClosePeriodAsync`, líneas 45–145)
  - `SBC.Application/Services/Implementation/BulkImportService.cs` (`ImportFromExcelAsync`, líneas 141–157)
- **Problema**:
  - `ClosePeriodAsync` mayoriza cada asiento llamando `journalEntryRepository.UpdateAsync(entry)` en un bucle, luego crea el asiento de cierre y finalmente actualiza/crea el período en llamadas `SaveChangesAsync` separadas. Un fallo intermedio deja asientos mayorizados sin cierre o asiento de cierre sin período marcado como cerrado.
  - `ImportFromExcelAsync` inserta cada asiento contable en llamadas `AddAsync` individuales y luego registra `BulkImport` aparte.
- **Acción recomendada**: Introducir una abstracción de Unit of Work / gestor de transacciones (`BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`) o persistencia atómica por lote para garantizar consistencia todo-o-nada.
- **Criterio de cierre**: Un error simulado a mitad de `ClosePeriodAsync` o `ImportFromExcelAsync` revierte completamente las escrituras previas.

### `TD-04` — Inconsistencias de configuración (`JwtSettings`, CORS) y campos de dominio nunca asignados
- **Estado**: `Pendiente`
- **Categoría**: Configuración / Consistencia
- **Ubicación**:
  - `SBC.Api/appsettings.json` y `SBC.Api/appsettings.Development.json` (clave `"Secret"`)
  - `SBC.Api/Program.cs` (líneas 54, 71, 97–98: `jwtSettings["SecretKey"]`, `secretKey!`, `// TODO leer de appSettings allowedOrigins`)
  - `SBC.Application/Services/Implementation/AuthService.cs` (`LoginAsync`, `LastLoginDate`)
  - `SBC.Application/Services/Implementation/AccountingPeriodService.cs` (`MarkAsClosed`, `ClosedBy`)
- **Problema**:
  1. `appsettings*.json` define `JwtSettings:Secret`, mientras el código lee `JwtSettings:SecretKey` (y usa `secretKey!` en `Program.cs`, causando `ArgumentNullException` poco claro si falta).
  2. CORS tiene `http://localhost:5173` hardcodeado pese a que `appsettings.json` ya declara `"AllowedOrigins": ""`.
  3. `ApplicationUser.LastLoginDate` existe en el modelo/BD pero `AuthService.LoginAsync` nunca lo actualiza.
  4. `AccountingPeriod.ClosedBy` existe en el modelo/BD pero `AccountingPeriodService.MarkAsClosed` nunca lo asigna con el usuario actual.
- **Acción recomendada**: Alinear `appsettings*.json` a `SecretKey` (con valor vacío), validar configuración requerida al arranque (`Options` + fail-fast), leer orígenes permitidos desde `AllowedOrigins` y poblar `LastLoginDate` y `ClosedBy`.
- **Criterio de cierre**: Configuración validada al iniciar, CORS configurable por entorno y campos `LastLoginDate`/`ClosedBy` persistidos y probados.

### `TD-05` — Manejo de excepciones no controladas en `SbcControllerBase` y uso de `new Exception(...)`
- **Estado**: `Pendiente`
- **Categoría**: Manejo de Errores / Seguridad
- **Ubicación**:
  - `SBC.Api/Controllers/Base/SbcControllerBase.cs` (líneas 65–80 y 145–160)
  - `SBC.Api/Controllers/AccountingPeriodsController.cs` (línea 30)
- **Problema**:
  1. `SbcControllerBase.ExecuteServiceAsync` captura cualquier `Exception` no controlada, devuelve `400 BadRequest` (en lugar de `500 InternalServerError`), expone `e.Message` crudo al cliente (riesgo de filtrar mensajes internos de SQL/EF Core) y no registra el stack trace con `ILogger`.
  2. `AccountingPeriodsController.GetByPeriod` lanza `new Exception("Periodo no encontrado")` en lugar de `NotFoundException`, devolviendo `400` en vez de `404`.
- **Acción recomendada**: Devolver `500` con mensaje genérico al cliente para excepciones inesperadas (registrando el detalle completo en `ILogger` / middleware global `IExceptionHandler`), y reemplazar `new Exception(...)` en `AccountingPeriodsController` por `NotFoundException`.
- **Criterio de cierre**: Excepciones inesperadas devuelven `500` sin filtrar detalles internos; `GetByPeriod` inexistente devuelve `404 NotFound`.

---

## Prioridad Media

### `TD-06` — Paquete preview obsoleto de EF Core en `SBC.Application.csproj` (`NU1603`)
- **Estado**: `Pendiente`
- **Categoría**: Dependencias / Arquitectura
- **Ubicación**: `SBC.Application/SBC.Application.csproj` (línea 17)
- **Problema**: `SBC.Application` referencia `Microsoft.EntityFrameworkCore 10.0.0-preview.2.25115.7` (que ya no existe en NuGet y genera advertencia `NU1603` resolviendo `10.0.0-preview.2.25163.8`), mientras `SBC.Api` y `SBC.Infrastructure` usan `10.0.5`. Además, `SBC.Application` solo usa EF Core en `UserService` (`CountAsync`/`ToListAsync` sobre `userManager.Users`).
- **Acción recomendada**: Alinear a `Microsoft.EntityFrameworkCore 10.0.5` o, preferiblemente para mayor pureza de capas, mover la consulta paginada de usuarios a un `IUserRepository` en `SBC.Infrastructure` y remover EF Core de `SBC.Application`.
- **Criterio de cierre**: Cero advertencias `NU1603` en `dotnet build SBC.sln` y versiones de EF Core alineadas.

### `TD-07` — Doble registro y brechas en auditoría `TransactionLog`
- **Estado**: `Pendiente`
- **Categoría**: Auditoría (`TransactionLog`)
- **Ubicación**:
  - Duplicación en: `JournalEntriesController.GetPaged` + `JournalEntryService.GetPagedAsync`; `JournalEntryLinesController.GetPaged` + `JournalEntryLineService.GetPagedAsync`; `BulkImportsController.GetPagedHistory` + `BulkImportService.GetPagedHistoryAsync`.
  - Omisiones en: `UsersController.GetUsers` y `UsersController.GetUserById` (sin `logAction`).
  - Falta de detalle en: `FinancialReportsController` (exportaciones Excel/PDF registran la misma acción que el reporte JSON sin indicar el formato exportado).
- **Problema**: Cada consulta paginada en los 3 endpoints afectados inserta **dos** filas en `logging.TransactionLogs` (una desde el servicio y otra desde `SbcControllerBase.ExecuteServiceAsync`), mientras las lecturas de `UsersController` no se auditan.
- **Acción recomendada**: Aplicar estrictamente la regla de registro único definida en `AGENTS.md` (remover `LogTransactionAsync` de los métodos GET paginados de servicio o dejar de pasar `logAction` en el controlador; agregar `TransactionActions.GetUsers` en `UsersController`; incluir `format` en los detalles de `FinancialReportService`).
- **Criterio de cierre**: Exactamente 1 registro de auditoría por cada operación HTTP, verificado mediante tests unitarios.

### `TD-08` — Cuellos de botella de rendimiento (N+1 y agregaciones históricas en memoria)
- **Estado**: `Pendiente`
- **Categoría**: Rendimiento
- **Ubicación**:
  - `SBC.Application/Services/Implementation/UserService.cs` (`GetPagedUsersAsync`, `GetUsersAsync`, líneas 178–201)
  - `SBC.Application/Services/Implementation/DashboardService.cs` (`GetSummaryAsync`, líneas 17–29)
  - `SBC.Application/Services/Implementation/FinancialReportService.cs` (`GetBalanceSheetAsync`, líneas 280–347)
  - `SBC.Application/Services/Implementation/BulkImportService.cs` (`ImportFromExcelAsync`, líneas 58–75)
- **Problema**:
  1. `UserService` ejecuta `userManager.GetRolesAsync(user)` dentro de un bucle por usuario (consultas N+1).
  2. `DashboardService` y `GetBalanceSheetAsync` cargan **todos** los asientos y líneas históricos desde `DateTime.MinValue` en memoria para luego sumar en C#, lo cual degradará memoria y latencia conforme crezca el libro diario.
  3. `BulkImportService` consulta `IsPeriodOpenAsync` y `GetByCodeAsync` en cada fila del Excel en lugar de precargar/cachear períodos y cuentas por lote.
- **Acción recomendada**: Implementar agregaciones por `AccountId`/`AccountType` directamente en SQL (proyección/GROUP BY en repositorio), resolver roles de usuarios con join en una sola consulta y cachear períodos/cuentas en un diccionario local durante `ImportFromExcelAsync`.
- **Criterio de cierre**: Eliminación de consultas N+1 y reducción drástica de filas transferidas en dashboard y Balance General.

### `TD-09` — Mezcla de `DateTime.Now` y `DateTime.UtcNow`
- **Estado**: `Pendiente`
- **Categoría**: Fechas / Zonas Horarias
- **Ubicación**:
  - `DateTime.Now`: `SbcDbContext.cs` (L51), `AccountingPeriodService.cs` (L195, L203), `DashboardService.cs` (L15)
  - `DateTime.UtcNow` / `DateTimeKind.Unspecified`: `AuthService.cs` (L118, L172, L234–L235), `TransactionLogService.cs` (L47)
- **Problema**: Las fechas de auditoría (`CreatedAt`, `UpdatedAt`, `ClosedAt`) dependen de la zona horaria del servidor/contenedor (Linux Docker suele estar en UTC, Windows local en hora local), mientras `TransactionLog.LogDate` y tokens usan UTC.
- **Acción recomendada**: Estandarizar toda la persistencia de timestamps a UTC (`DateTime.UtcNow` o `DateTimeOffset.UtcNow`) y centralizar el reloj con `TimeProvider` de .NET para facilitar pruebas determinísticas.
- **Criterio de cierre**: Uso consistente de UTC en toda la solución sin mezclas de `DateTime.Now` vs `DateTime.UtcNow`.

### `TD-10` — Brechas de validación de entrada, paginación y reglas de dominio menores
- **Estado**: `Pendiente`
- **Categoría**: Validación / Dominio
- **Ubicación**:
  - `SBC.Application.Models/Common/BaseFilterDto.cs` y `PagedResultDto.cs`
  - `SBC.Api/Controllers/UsersController.cs` (L83–L96), `AccountsController.cs` (`GetAll`), `AccountingPeriodsController.cs` (`GetAll`)
  - `SBC.Application/Services/Implementation/AccountingPeriodService.cs` (`ClosePeriodAsync`, L31–L37 y L52)
  - `SBC.Infrastructure/Database/Configurations/JournalEntryLineConfiguration.cs` (L34–L35)
  - `SBC.Domain.Entities/Base/BaseEntity.cs` (L30: `//TODO: createdBy and updatedBy should be Guid?`)
- **Problema**:
  1. `BaseFilterDto` no valida `PageNumber >= 1` ni límites de `PageSize` (si `PageSize <= 0`, `PagedResultDto.TotalPages` lanza `DivideByZeroException` o produce valores inválidos).
  2. `UsersController.GetUsers` permite saltar la paginación con `PageSize == 0 && PageNumber == 0`; `AccountsController.GetAll` y `AccountingPeriodsController.GetAll` no exponen endpoints paginados.
  3. `AccountingPeriodService.ClosePeriodAsync` valida que `equityAccountId` exista, pero no valida que `equityAccount.Type == AccountType.Equity`, ni pasa explícitamente `includeUnposted: false` (depende implícitamente del valor por defecto).
  4. `JournalEntryLineConfiguration` tiene comentada la restricción `CK_JournalEntryLines_Exclusive`, permitiendo una misma línea con `Debit > 0` y `Credit > 0` simultáneamente.
  5. `BaseEntity` mantiene pendiente evaluar si `CreatedBy`/`UpdatedBy` deben evolucionar a `Guid?` o permanecer como `string?` (username).
- **Acción recomendada**: Agregar validación de rangos en `BaseFilterDto` (y proteger `TotalPages`), validar `AccountType.Equity` en el cierre de período, definir política de negocio para `CK_JournalEntryLines_Exclusive` y documentar/decidir el tipo definitivo de `CreatedBy`/`UpdatedBy`.
- **Criterio de cierre**: Entradas inválidas de paginación y cuenta de patrimonio de tipo incorrecto rechazadas con `400 BadRequest` + pruebas unitarias.

### `TD-11` — Cobertura faltante de pruebas unitarias e integración
- **Estado**: `Pendiente`
- **Categoría**: Pruebas (`SBC.UnitTest`)
- **Ubicación**: `SBC.UnitTest/Controllers/` y `SBC.UnitTest/Services/`
- **Problema**:
  - **5 de 11 controladores sin tests**: `BulkImportsController`, `DashboardController`, `FinancialReportsController`, `TransactionLogsController`, `UsersController`.
  - **2 de 10 servicios sin tests**: `DashboardService`, `TransactionLogService`.
  - `BulkImportServiceTests` aún no cubre `ImportFromExcelAsync` (casos válidos, partida doble inválida, período cerrado, cuenta inexistente, archivo sin hojas) ni `GetPagedHistoryAsync`.
  - No existen pruebas de integración para verificar consultas LINQ/EF Core (`GetPagedAsync`, `GetTopAccountsAsync`, constraints SQL y `SbcDbContext.SaveChangesAsync`).
- **Acción recomendada**: Completar pruebas unitarias para los controladores y servicios faltantes, ampliar `BulkImportServiceTests` y evaluar un proyecto de pruebas de integración (`SBC.IntegrationTest`).
- **Criterio de cierre**: 100% de controladores y servicios con suite de pruebas unitarias activas y pasando en `dotnet test`.

---

## Prioridad Baja

### `TD-12` — 19 advertencias activas del compilador C# (`CS8618`, `CS9107`, `CS8619`, `CS8600`)
- **Estado**: `Pendiente`
- **Categoría**: Calidad de Código / Nullability
- **Ubicación**:
  - `CS8618` (15 avisos): `Account.cs`, `JournalEntry.cs`, `JournalEntryLine.cs`, `BalanceSheetDtos.cs`, `DashboardDtos.cs`, `IncomeStatementDtos.cs`.
  - `CS9107` y `CS8619` (2 avisos): `SBC.Infrastructure/Repositories/Implementation/TransactionLogRepository.cs` (L9: parámetro `context` capturado duplicado respecto a `_context` de `BaseRepository`; L48: nulabilidad de `Email`).
  - `CS8600` (2 avisos): `SBC.UnitTest/Services/AccountingPeriodServiceTests.cs` (L40, L89: cast `(AccountingPeriod)null` en lugar de `(AccountingPeriod?)null`).
- **Problema**: Con `<Nullable>enable</Nullable>`, las propiedades sin inicializar o casts no anulables generan ruido constante en cada build y pueden ocultar verdaderos riesgos de `NullReferenceException`.
- **Acción recomendada**: Inicializar strings/navegaciones adecuadamente (`= string.Empty;`, `= null!;` o anulables `?`), usar `_context` heredado en `TransactionLogRepository` y corregir los casts anulables en tests.
- **Criterio de cierre**: `dotnet build SBC.sln` con `0` advertencias C# de nulabilidad/constructor.

### `TD-13` — `Dockerfile` incompleto para restore multi-proyecto y endpoints faltantes en `SBC.Api.http`
- **Estado**: `Pendiente`
- **Categoría**: DevEx / Contenedores
- **Ubicación**: `SBC.Api/Dockerfile` (líneas 10–11), `SBC.Api/SBC.Api.http`
- **Problema**:
  1. `Dockerfile` copia únicamente `SBC.Api/SBC.Api.csproj` antes de ejecutar `dotnet restore`, lo cual falla al intentar restaurar los proyectos referenciados (`SBC.Infrastructure`, `SBC.Application`, `SBC.Application.Models`, `SBC.Domain`, `SBC.Domain.Entities`) antes de `COPY . .`.
  2. `SBC.Api/SBC.Api.http` no contiene ejemplos para los 4 endpoints de `FinancialReportsController` (`/api/financialReports/...`) ni para `GET /api/users/{userId}`, además de contener datos de ejemplo con nombres reales en el registro/login inicial.
- **Acción recomendada**: Copiar todos los `.csproj` necesarios antes de `dotnet restore` en `Dockerfile`, agregar los endpoints faltantes a `SBC.Api.http` y anonimizar los datos de ejemplo.
- **Criterio de cierre**: `docker build -f SBC.Api/Dockerfile .` construye exitosamente y todos los endpoints de la API tienen ejemplo en `SBC.Api.http`.

### `TD-14` — Higiene menor de código y ortografía en catálogo semilla
- **Estado**: `Pendiente`
- **Categoría**: Higiene de Código
- **Ubicación**:
  - `SBC.Application/Services/Implementation/JournalEntryService.cs` (L9: `using ClosedXML.Excel;` sin usar)
  - `SBC.Application/Services/Implementation/AccountingPeriodService.cs` (L211: `GenerateClosingCodeAsync` declarada `async` sin `await`)
  - `SBC.Application/Services/Implementation/AccountService.cs` y `SBC.Api/Controllers/AccountsController.cs` (aún usan constructor tradicional en lugar de primary constructor)
  - `SBC.Infrastructure/Database/Seeder/AccountSeeder.cs` (erratas en nombres de cuentas: `"IVA CREDITO TRIBUNAL"`, `"DEPRECIACION ACUMULDA"`, `"DEPOSITOD ENTREGADO"`, `"PROVICIONES POR PAGAR"`, `"PRESTMOS SOCIOS"`, `"CONS. DE ENEGIA"`, etc.)
- **Problema**: Inconsistencias menores acumuladas durante la evolución del proyecto. *(Nota: corregir `AccountSeeder.cs` modificará datos semilla y requerirá generar su migración EF Core correspondiente).*
- **Acción recomendada**: Limpiar código muerto/estilístico en tareas de mantenimiento y evaluar con negocio la corrección de textos del catálogo contable semilla junto con su migración EF Core.
- **Criterio de cierre**: Código limpio sin `using`/métodos `async` innecesarios y decisión aplicada sobre nombres en `AccountSeeder`.
