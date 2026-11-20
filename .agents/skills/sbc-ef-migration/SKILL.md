---
name: sbc-ef-migration
description: >-
  Runbook para cambios en el modelo de datos de Entity Framework Core,
  configuraciones Fluent API, seeders determinísticos y migraciones SQL Server
  en SBC. Úsala siempre que se agreguen o modifiquen entidades en
  SBC.Domain.Entities, configuraciones/seeders en SBC.Infrastructure/Database o
  esquemas de base de datos.
---

# Cambios de Esquema, Configuraciones y Migraciones EF Core en SBC

## 1. Reglas de Entidades y `SbcDbContext`

- **Herencia de `BaseEntity`**: Toda entidad persistente (excepto `ApplicationUser`, que hereda de `IdentityUser<Guid>`) debe heredar de `BaseEntity` (`Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`).
- **Auditoría automática**: `SbcDbContext.SaveChangesAsync` (`SBC.Infrastructure/Database/SbcDbContext.cs`) rellena automáticamente `CreatedAt`/`CreatedBy` al insertar y `UpdatedAt`/`UpdatedBy` al modificar usando `DateTime.Now` y `httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system"`. No asignes ni sobreescribas estas propiedades en servicios.
- **DbSet**: Declara `public DbSet<MiEntidad> MiEntidades { get; set; }` en `SbcDbContext`.
- **Query Splitting**: `Program.cs` configura `QuerySplittingBehavior.SplitQuery` globalmente; no dupliques esta configuración innecesariamente por consulta.

## 2. Convenciones de `IEntityTypeConfiguration<T>`

Crea una clase `<Entidad>Configuration : IEntityTypeConfiguration<Entidad>` en `SBC.Infrastructure/Database/Configurations/`. `SbcDbContext.OnModelCreating` las descubre automáticamente mediante `ApplyConfigurationsFromAssembly`.

Verifica cada uno de estos puntos:
- **Esquema SQL correcto**:
  - `accounting`: entidades contables (`Accounts`, `AccountingPeriods`, `BulkImports`, `JournalEntries`, `JournalEntryLines`).
  - `identity`: entidades de ASP.NET Core Identity (configuradas directamente en `SbcDbContext.OnModelCreating`).
  - `logging`: auditoría (`TransactionLogs`).
- **Claves primarias**:
  - En el esquema `accounting`, configura explícitamente `builder.HasKey(x => x.Id);` y `builder.Property(x => x.Id).ValueGeneratedNever();` (los servicios asignan `Guid.NewGuid()`).
- **Tipos de columna y longitudes explícitas**:
  - Montos monetarios (`Debit`, `Credit`): `HasColumnType("money")`.
  - Fechas: `HasColumnType("datetime2")`.
  - Enteros pequeños (`Year`, `Month`, `Day`): `HasColumnType("smallint")`.
  - Strings: define siempre `HasMaxLength(...)` y `HasColumnType("varchar(...)")` o `HasColumnType("nvarchar(...)")` según corresponda.
  - Enums (`AccountType`, `TransactionStatus`): almacénalos como texto con `.HasConversion<string>().HasMaxLength(...)`.
- **Restricciones e integridad referencial**:
  - Agrega `HasCheckConstraint(...)` dentro de `builder.ToTable(t => { ... })` para invariantes de base de datos (rangos de año/mes/día, valores no negativos, strings no vacíos, enums válidos).
  - Define explícitamente `OnDelete(...)`:
    - `DeleteBehavior.Cascade` solo para agregados padre-hijo estrictos (como `JournalEntry` → `JournalEntryLine`).
    - `DeleteBehavior.Restrict` para referencias de catálogo o historial (`Account.ParentAccount`, `JournalEntryLine.Account`, `AccountingPeriod.ClosingJournalEntry`).
- **Índices de rendimiento**:
  - Agrega `HasIndex(...)` en campos de búsqueda frecuente, claves de negocio únicas (`IsUnique()`) y columnas filtradas/ordenadas en consultas paginadas.

## 3. Reglas de Seeders (`SBC.Infrastructure/Database/Seeder/`)

Los seeders actuales (`RolSeeder`, `UserSeeder`, `AccountSeeder`) se ejecutan vía `HasData` en `OnModelCreating`:
- **Determinismo estricto**: Todos los `Id`, `ConcurrencyStamp`, `SecurityStamp`, `PasswordHash` y fechas (`CreatedAt`) deben ser valores constantes o generados de forma determinística (ej. `AccountSeeder.GenerateGuid(code)`).
- **Prohibido** usar `Guid.NewGuid()`, `DateTime.Now` o `DateTime.UtcNow` dentro de `HasData`, ya que provocan cambios fantasma en cada migración.

## 4. Generación y Revisión de la Migración

1. Verifica que la solución compila antes de generar la migración:
   ```bash
   dotnet build SBC.sln
   ```
2. Genera la migración especificando siempre proyecto de migraciones y proyecto de inicio:
   ```bash
   dotnet ef migrations add <NombreDescriptivo> --project SBC.Infrastructure --startup-project SBC.Api
   ```
   *(Usa nombres descriptivos en PascalCase como `AddBudgetTable` o `AddJournalEntryIndexes`; evita nombres genéricos como `PendingChanges`.)*
3. **Inspección obligatoria de la migración generada** (`SBC.Infrastructure/Migrations/<Timestamp>_<Nombre>.cs`):
   - Revisa `Up()` y `Down()` buscando `DropTable`, `DropColumn` o cambios inesperados en seeders/FKs que puedan causar pérdida de datos.
   - Si una operación es destructiva en datos existentes, confirma explícitamente con el usuario antes de aplicarla.
4. Valida compilación y pruebas tras generar la migración:
   ```bash
   dotnet build SBC.sln
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj
   ```
5. Si el usuario solicita aplicar la migración localmente:
   ```bash
   dotnet ef database update --project SBC.Infrastructure --startup-project SBC.Api
   ```

## 5. Mantenimiento de Documentación de Agentes

Si el cambio agrega nuevas tablas/esquemas, seeders o reglas estructurales de persistencia, actualiza `AGENTS.md`, esta skill (`sbc-ef-migration`) y sus copias espejo en `docs/agent-guidelines-en/`.
