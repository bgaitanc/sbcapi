---
name: sbc-unit-testing
description: >-
  Patrones y convenciones para escribir, actualizar y ejecutar pruebas unitarias
  con xUnit, Moq y MockQueryable.Moq en SBC.UnitTest. Úsala al agregar pruebas
  para controladores o servicios, o al diagnosticar y corregir tests fallidos.
---

# Pruebas Unitarias en SBC (`SBC.UnitTest`)

## 1. Estructura y Convenciones Generales

- **Ubicación**:
  - Pruebas de controladores: `SBC.UnitTest/Controllers/<Nombre>ControllerTests.cs` (`namespace SBC.UnitTest.Controllers;`).
  - Pruebas de servicios: `SBC.UnitTest/Services/<Nombre>ServiceTests.cs` (`namespace SBC.UnitTest.Services;`).
- **Herramientas**: `xUnit` (`[Fact]` / `[Theory]`, `Assert.*`), `Moq` (`Mock<T>`) y `MockQueryable.Moq` (`.BuildMock()`).
- **Patrón AAA**: Estructura cada prueba con comentarios `// Arrange`, `// Act`, `// Assert` (o `// Act & Assert` para excepciones).
- **Nomenclatura**: `<Metodo>_Should<ResultadoEsperado>[_When<Condicion>]` o `<Metodo>_When<Condicion>_Should<ResultadoEsperado>`.

## 2. Patrón para Pruebas de Controladores (`SbcControllerBase`)

Instancia el controlador directamente pasando `Mock<IService>.Object`:
- No es necesario inicializar `ControllerContext`/`HttpContext` en tests unitarios básicos porque `SbcControllerBase.GetLogService()` usa acceso null-safe (`HttpContext?.RequestServices?.GetService(...)`) y omite el logging cuando no hay `HttpContext`.
- **Desempaquetado de respuestas de `ExecuteServiceAsync`**:
  - `200 OK` (con datos):
    ```csharp
    var actionResult = Assert.IsType<ActionResult<MyDto>>(result);
    var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    var response = Assert.IsType<SbcGenericResponse<MyDto>>(objectResult.Value);
    Assert.True(response.Success);
    Assert.Equal(expected, response.Data);
    ```
  - `201 Created`:
    ```csharp
    var actionResult = Assert.IsType<ActionResult<MyDto>>(result);
    var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
    var response = Assert.IsType<SbcGenericResponse<MyDto>>(objectResult.Value);
    Assert.True(response.Success);
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    ```
  - `204 NoContent` (`PUT`/`DELETE` sobre `ExecuteServiceAsync(Func<Task>)`):
    ```csharp
    var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
    Assert.IsType<NoContentResult>(actionResult.Result);
    ```
  - Respuestas binarias (`FinancialReportsController`, `BulkImportsController.GetTemplate`): valida `Assert.IsType<FileContentResult>(result)`, `ContentType`, `FileDownloadName` y `FileContents`.

## 3. Patrones y Trampas Comunes al Mockear Servicios

1. **Mock de `ITransactionLogService`**:
   - Todos los servicios que auditan reciben `ITransactionLogService`. Declara `private readonly Mock<ITransactionLogService> _transactionLogServiceMock = new();` y pásalo al constructor. Verifica `LogTransactionAsync` cuando el comportamiento de auditoría sea parte del requerimiento.
2. **Mock de `IAccountingPeriodRepository` (asientos y carga masiva)**:
   - En Moq, `bool` devuelve `false` por defecto. En pruebas de `JournalEntryService` o `BulkImportService`, configura en el constructor de la clase de pruebas:
     ```csharp
     _accountingPeriodRepositoryMock
         .Setup(r => r.IsPeriodOpenAsync(It.IsAny<int>(), It.IsAny<int>()))
         .ReturnsAsync(true);
     ```
   - Sobreescríbelo a `ReturnsAsync(false)` específicamente en el test que verifica rechazo por período cerrado.
3. **Parámetros opcionales en Moq (`includeUnposted`)**:
   - `IJournalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate, bool includeUnposted = false)` tiene un parámetro opcional. Al hacer `Setup`, haz coincidir exactamente el valor `bool` (`false` o `true`, o usa `It.IsAny<bool>()` si no es el foco del test), ya que Moq distingue `false` vs `true`.
4. **Mock de `UserManager<ApplicationUser>` y consultas async (`Users`)**:
   - Construye `UserManager` con un `Mock<IUserStore<ApplicationUser>>`:
     ```csharp
     var store = new Mock<IUserStore<ApplicationUser>>();
     _userManagerMock = new Mock<UserManager<ApplicationUser>>(
         store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
     ```
   - Para pruebas que ejecuten `CountAsync()`/`ToListAsync()` sobre `userManager.Users`, usa `MockQueryable.Moq`:
     ```csharp
     var mockUsers = userList.AsQueryable().BuildMock();
     _userManagerMock.Setup(m => m.Users).Returns(mockUsers);
     ```
5. **Validación de excepciones de dominio**:
   - Usa `await Assert.ThrowsAsync<DomainException>(...)`, `NotFoundException` o `SbcException` y verifica `exception.StatusCode` y/o `exception.Message`.

## 4. Comandos de Ejecución

```bash
# Ejecutar toda la suite
dotnet test SBC.UnitTest/SBC.UnitTest.csproj

# Ejecutar una clase de pruebas específica
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~AccountsControllerTests"

# Ejecutar un método de prueba específico
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntryServiceTests.CreateAsync_ShouldThrowException_WhenPeriodIsClosed"

# Ejecutar con salida detallada
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --verbosity normal
```

## 5. Mantenimiento de Documentación de Agentes

Si agregas nuevas librerías de testing (por ejemplo, integración con WebApplicationFactory o Testcontainers) o cambias patrones base de pruebas, actualiza `AGENTS.md`, esta skill (`sbc-unit-testing`) y sus copias espejo en `docs/agent-guidelines-en/`.
