---
name: sbc-unit-testing
description: >-
  Patterns and conventions for writing, updating, and running xUnit, Moq, and
  MockQueryable.Moq unit tests in SBC.UnitTest. Use when adding controller or
  service tests, or when diagnosing and fixing failing tests.
---

# Unit Testing Patterns in SBC (`SBC.UnitTest`)

## 1. Structure & General Conventions

- **Locations**:
  - Controller tests: `SBC.UnitTest/Controllers/<Name>ControllerTests.cs` (`namespace SBC.UnitTest.Controllers;`).
  - Service tests: `SBC.UnitTest/Services/<Name>ServiceTests.cs` (`namespace SBC.UnitTest.Services;`).
- **Libraries**: `xUnit` (`[Fact]` / `[Theory]`, `Assert.*`), `Moq` (`Mock<T>`), and `MockQueryable.Moq` (`.BuildMock()`).
- **AAA pattern**: Structure every test with `// Arrange`, `// Act`, `// Assert` (or `// Act & Assert` for exception assertions).
- **Naming**: `<Method>_Should<ExpectedOutcome>[_When<Condition>]` or `<Method>_When<Condition>_Should<ExpectedOutcome>`.

## 2. Testing `SbcControllerBase` Controllers

Instantiate the controller directly with `Mock<IService>.Object`:
- You do not need to set up `ControllerContext`/`HttpContext` for standard controller tests because `SbcControllerBase.GetLogService()` is null-safe (`HttpContext?.RequestServices?.GetService(...)`) and skips logging when `HttpContext` is null.
- **Unwrapping `ExecuteServiceAsync` results**:
  - `200 OK` (with payload):
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
  - `204 NoContent` (`PUT`/`DELETE` over `ExecuteServiceAsync(Func<Task>)`):
    ```csharp
    var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
    Assert.IsType<NoContentResult>(actionResult.Result);
    ```
  - Binary responses (`FinancialReportsController`, `BulkImportsController.GetTemplate`): assert `Assert.IsType<FileContentResult>(result)`, `ContentType`, `FileDownloadName`, and `FileContents`.

## 3. Common Service Mocking Patterns & Pitfalls

1. **`ITransactionLogService` mock**:
   - Services that audit accept `ITransactionLogService`. Declare `private readonly Mock<ITransactionLogService> _transactionLogServiceMock = new();` and inject it. Verify `LogTransactionAsync` calls whenever auditing behavior is part of the requirement.
2. **`IAccountingPeriodRepository` mock (journal entries & bulk imports)**:
   - Moq returns `false` for `bool` by default. In `JournalEntryServiceTests` or `BulkImportServiceTests`, configure an open period by default in the test constructor:
     ```csharp
     _accountingPeriodRepositoryMock
         .Setup(r => r.IsPeriodOpenAsync(It.IsAny<int>(), It.IsAny<int>()))
         .ReturnsAsync(true);
     ```
   - Override to `ReturnsAsync(false)` inside the specific test verifying closed-period rejection.
3. **Optional parameters in Moq (`includeUnposted`)**:
   - `IJournalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate, bool includeUnposted = false)` has an optional parameter. Match the exact `bool` (`false` vs `true`, or `It.IsAny<bool>()` when irrelevant) in `Setup(...)`, because Moq differentiates optional argument values.
4. **Mocking `UserManager<ApplicationUser>` and async `Users` queries**:
   - Construct `UserManager` with a mocked `IUserStore<ApplicationUser>`:
     ```csharp
     var store = new Mock<IUserStore<ApplicationUser>>();
     _userManagerMock = new Mock<UserManager<ApplicationUser>>(
         store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
     ```
   - For tests executing `CountAsync()`/`ToListAsync()` against `userManager.Users`, use `MockQueryable.Moq`:
     ```csharp
     var mockUsers = userList.AsQueryable().BuildMock();
     _userManagerMock.Setup(m => m.Users).Returns(mockUsers);
     ```
5. **Domain exception assertions**:
   - Use `await Assert.ThrowsAsync<DomainException>(...)`, `NotFoundException`, or `SbcException` and verify `exception.StatusCode` and/or `exception.Message`.

## 4. Test Execution Commands

```bash
# Run full test suite
dotnet test SBC.UnitTest/SBC.UnitTest.csproj

# Run a specific test class
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~AccountsControllerTests"

# Run a single test method
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntryServiceTests.CreateAsync_ShouldThrowException_WhenPeriodIsClosed"

# Run with detailed output
dotnet test SBC.UnitTest/SBC.UnitTest.csproj --verbosity normal
```

## 5. Agent Documentation Maintenance

If new test libraries (e.g., WebApplicationFactory or Testcontainers) are introduced or core testing conventions change, update `AGENTS.md`, this skill (`sbc-unit-testing`), and their mirrors in `docs/agent-guidelines-en/`.
