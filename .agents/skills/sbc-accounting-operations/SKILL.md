---
name: sbc-accounting-operations
description: >-
  Reglas de negocio contables, validaciones de período, partida doble, cierre
  de período, importación masiva Excel (ClosedXML) y generación de reportes
  financieros JSON/Excel/PDF (QuestPDF) en SBC. Úsala al modificar o extender
  Account, AccountingPeriod, JournalEntry, JournalEntryLine, BulkImport,
  FinancialReportService, DashboardService, ReportsController o
  FinancialReportsController.
---

# Reglas y Procedimientos de Operaciones Contables en SBC

## 1. Invariantes Contables Fundamentales

Verifica siempre estos invariantes antes de persistir cambios en el módulo contable:

1. **Período contable abierto obligatorio**:
   - Antes de crear o editar un `JournalEntry` (manual en `JournalEntryService` o masivo en `BulkImportService`), valida:
     ```csharp
     await accountingPeriodRepository.IsPeriodOpenAsync(date.Year, date.Month)
     ```
   - Si devuelve `false`, rechaza la operación (lanza `DomainException` en creación/edición individual; registra error de fila en importación masiva).
2. **Principio de partida doble**:
   - Todo `JournalEntry` debe cumplir `entry.ValidateDoubleEntry()` (`Lines.Sum(x => x.Debit) == Lines.Sum(x => x.Credit)`).
   - Al crear, editar o eliminar una línea individual (`JournalEntryLineService`), valida el balance resultante del asiento padre antes de guardar (permitiendo eliminar la última línea solo si `entry.Lines.Count == 0`).
   - En base de datos, cada línea exige `Debit >= 0`, `Credit >= 0` y `Debit > 0 OR Credit > 0`.
3. **Inmutabilidad de asientos mayorizados (`IsPosted == true`)**:
   - Un `JournalEntry` con `IsPosted == true` **no puede** editarse ni eliminarse, ni pueden agregarse, editarse o eliminarse sus `JournalEntryLine`. Lanza `DomainException`.
4. **Desglose de fecha y códigos de asiento**:
   - Mantén siempre sincronizados `Date`, `Day = date.Day`, `Month = date.Month` y `Year = date.Year`.
   - Formato de código regular: `{year}-{month:D2}-{nextNumber:D4}` (secuencial por año/mes usando `GetLastCodeAsync`, y en carga masiva considerando también los asientos en memoria pendientes de guardar).
   - Formato de código de cierre: `CIE-{year}-{month:D2}`.
5. **Catálogo de cuentas (`Account`)**:
   - `Code` y `Name` son únicos.
   - No se puede eliminar una cuenta que tenga `SubAccounts`.
   - El árbol (`AccountService.GetTreeAsync`) se construye en memoria a partir de `GetAllAsync()` para soportar N niveles sin depender de profundidad fija de `Include`.

## 2. Naturaleza de Saldos y Fórmulas Financieras

Usa exactamente estas convenciones de signo según `AccountType` (`SBC.Domain.Entities.Enums.AccountType`):

| `AccountType` | Estado Financiero | Naturaleza | Fórmula de saldo |
| :--- | :--- | :--- | :--- |
| `Asset` (`1`) | Balance General | Deudora | `Sum(Debit - Credit)` |
| `Liability` (`2`) | Balance General | Acreedora | `Sum(Credit - Debit)` |
| `Equity` (`3`) | Balance General | Acreedora | `Sum(Credit - Debit)` |
| `Revenue` (`4`) | Estado de Resultados | Acreedora | `Sum(Credit - Debit)` |
| `Cost` (`5`) | Estado de Resultados | Deudora | `Sum(Debit - Credit)` |
| `Expense` (`6`) | Estado de Resultados | Deudora | `Sum(Debit - Credit)` |

- **Estado de Resultados (`IncomeStatementDto`)**:
  - `GrossProfit = TotalRevenues - TotalCosts`
  - `NetIncome = GrossProfit - TotalExpenses`
- **Balance General (`BalanceSheetDto`)**:
  - Consulta asientos desde `DateTime.MinValue` hasta `date`.
  - Incluye `NetIncome` histórico calculado de cuentas `Revenue`, `Cost` y `Expense` hasta `date`.
  - `TotalLiabilitiesAndEquity = TotalLiabilities + TotalEquity + NetIncome` (debe cuadrar con `TotalAssets`).
- Omite en el detalle de líneas las cuentas cuyo saldo acumulado sea `0`.

## 3. Cierre de Período Contable (`AccountingPeriodService.ClosePeriodAsync`)

Al modificar el flujo de cierre, preserva esta secuencia:
1. Verificar que el período (`year`, `month`) no esté ya cerrado (`DomainException`).
2. Verificar que la cuenta de patrimonio destino (`equityAccountId`) exista (`NotFoundException`).
3. Obtener todos los asientos del mes con `GetByDateRangeWithLinesAsync(startDate, endDate, includeUnposted: true)` y mayorizar (`IsPosted = true`) los que aún no lo estén.
4. Calcular el Estado de Resultados del período (`GetIncomeStatementAsync(startDate, endDate)`). Si no hay movimientos ni `NetIncome`, marcar el período como cerrado sin asiento de cierre.
5. Construir el asiento de cierre (`IsPosted = true`, código `CIE-{year}-{month:D2}`, fecha `endDate`):
   - Debitar cada cuenta de `Revenues`.
   - Acreditar cada cuenta de `Costs` y `Expenses`.
   - Llevar la diferencia (`NetIncome`) a `equityAccountId`: crédito si `NetIncome > 0` (utilidad), débito por `Math.Abs(NetIncome)` si `NetIncome < 0` (pérdida).
   - Validar `closingEntry.ValidateDoubleEntry()` antes de guardar.
6. Marcar/crear el `AccountingPeriod` como cerrado (`IsClosed = true`, `ClosedAt`, `ClosingJournalEntryId`) y registrar el `TransactionLog`.

## 4. Reportes Provisionales y Exportación (JSON, Excel y PDF)

- **Dualidad de controladores**:
  - `ReportsController` (`/api/reports/income-statement`, `/api/reports/balance-sheet`) → respuestas JSON envueltas en `SbcGenericResponse<T>`.
  - `FinancialReportsController` (`/api/financialReports/.../excel`, `.../pdf`) → descargas binarias `File(...)` con tipos MIME `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` y `application/pdf`.
- **Bandera `includeUnposted`**:
  - Todos los endpoints de reportes financieros deben propagar `bool includeUnposted = false` hasta `IJournalEntryRepository.GetByDateRangeWithLinesAsync(..., includeUnposted)`.
  - Cuando `includeUnposted == true`, marcar `IsProvisional = true` en el DTO y mostrar la etiqueta `(Provisional)` / `PROVISIONAL` en las salidas Excel y PDF.
  - `DashboardService.GetSummaryAsync` siempre usa `includeUnposted = true` para reflejar el estado contable en tiempo real.
- **QuestPDF**: Mantén configurado `QuestPDF.Settings.License = LicenseType.Community;` en el constructor estático de `FinancialReportService` (o en el inicializador equivalente si se refactoriza).

## 5. Carga Masiva desde Excel (`BulkImportService` + ClosedXML)

Mantén estrictamente sincronizadas las columnas leídas en `ImportFromExcelAsync` y las generadas en `GenerateBulkImportTemplateAsync`:

| Columna | Índice | Contenido |
| :---: | :---: | :--- |
| A | `1` | Fecha (`DateTime`) |
| B | `2` | Descripción (`string`) |
| C | `3` | Código de Cuenta (`string`, validado con `IAccountRepository.GetByCodeAsync`) |
| D | `4` | Debe (`decimal`) |
| E | `5` | Haber (`decimal`) |
| F | `6` | Referencia de Grupo opcional (`string`; si viene vacía, agrupa por `{date:yyyyMMdd}-{description}`) |

- Registra siempre el resumen en la tabla `accounting.BulkImports` y audita en `TransactionLog` usando:
  - `TransactionStatus.Success` si `ErrorCount == 0`
  - `TransactionStatus.PartialSuccess` si `ErrorCount > 0 && SuccessCount > 0`
  - `TransactionStatus.Failure` si `SuccessCount == 0 && ErrorCount > 0`

## 6. Verificación y Mantenimiento de Documentación

1. Ejecuta pruebas relevantes tras cualquier cambio:
   ```bash
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntry|FullyQualifiedName~AccountingPeriod|FullyQualifiedName~FinancialReport|FullyQualifiedName~BulkImport"
   ```
2. Si modificaste fórmulas contables, estructura de columnas Excel, formatos de código o flujos de cierre/reportes, actualiza `AGENTS.md`, esta skill (`sbc-accounting-operations`) y sus espejos en `docs/agent-guidelines-en/`.
