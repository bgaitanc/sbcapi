---
name: sbc-accounting-operations
description: >-
  Core accounting business rules, period validations, double-entry invariants,
  period closing, Excel bulk import (ClosedXML), and JSON/Excel/PDF financial
  reporting (QuestPDF) in SBC. Use when modifying or extending Account,
  AccountingPeriod, JournalEntry, JournalEntryLine, BulkImport,
  FinancialReportService, DashboardService, ReportsController, or
  FinancialReportsController.
---

# Accounting Operations Rules & Procedures in SBC

## 1. Core Accounting Invariants

Always enforce these invariants before persisting accounting changes:

1. **Open accounting period required**:
   - Before creating or editing any `JournalEntry` (manual in `JournalEntryService` or bulk in `BulkImportService`), check:
     ```csharp
     await accountingPeriodRepository.IsPeriodOpenAsync(date.Year, date.Month)
     ```
   - When `false`, reject the operation (throw `DomainException` for manual operations; append a row error for bulk import).
2. **Double-entry bookkeeping**:
   - Every `JournalEntry` must satisfy `entry.ValidateDoubleEntry()` (`Lines.Sum(x => x.Debit) == Lines.Sum(x => x.Credit)`).
   - When creating, updating, or deleting an individual line (`JournalEntryLineService`), validate the resulting parent entry balance before saving (deleting the last line is allowed only if `entry.Lines.Count == 0`).
   - Database constraints also require `Debit >= 0`, `Credit >= 0`, and `Debit > 0 OR Credit > 0`.
3. **Immutability of posted entries (`IsPosted == true`)**:
   - A posted `JournalEntry` **cannot** be edited or deleted, nor can its `JournalEntryLine` items be added, updated, or removed. Throw `DomainException`.
4. **Date breakdown & entry code formats**:
   - Keep `Date`, `Day = date.Day`, `Month = date.Month`, and `Year = date.Year` synchronized.
   - Regular entry code format: `{year}-{month:D2}-{nextNumber:D4}` (sequential per year/month via `GetLastCodeAsync`, and in bulk import also accounting for unsaved in-memory entries).
   - Closing entry code format: `CIE-{year}-{month:D2}`.
5. **Chart of accounts (`Account`)**:
   - `Code` and `Name` are unique.
   - Accounts with `SubAccounts` cannot be deleted.
   - The hierarchy (`AccountService.GetTreeAsync`) is assembled in memory from `GetAllAsync()` to support N levels without fixed EF `Include` depth.

## 2. Normal Balances & Financial Formulas

Follow these exact sign conventions per `AccountType` (`SBC.Domain.Entities.Enums.AccountType`):

| `AccountType` | Financial Statement | Normal Balance | Balance Formula |
| :--- | :--- | :--- | :--- |
| `Asset` (`1`) | Balance Sheet | Debit | `Sum(Debit - Credit)` |
| `Liability` (`2`) | Balance Sheet | Credit | `Sum(Credit - Debit)` |
| `Equity` (`3`) | Balance Sheet | Credit | `Sum(Credit - Debit)` |
| `Revenue` (`4`) | Income Statement | Credit | `Sum(Credit - Debit)` |
| `Cost` (`5`) | Income Statement | Debit | `Sum(Debit - Credit)` |
| `Expense` (`6`) | Income Statement | Debit | `Sum(Debit - Credit)` |

- **Income Statement (`IncomeStatementDto`)**:
  - `GrossProfit = TotalRevenues - TotalCosts`
  - `NetIncome = GrossProfit - TotalExpenses`
- **Balance Sheet (`BalanceSheetDto`)**:
  - Queries entries from `DateTime.MinValue` through `date`.
  - Computes historical `NetIncome` from `Revenue`, `Cost`, and `Expense` accounts through `date`.
  - `TotalLiabilitiesAndEquity = TotalLiabilities + TotalEquity + NetIncome` (must balance `TotalAssets`).
- Omit zero-balance accounts from statement line lists.

## 3. Period Closing (`AccountingPeriodService.ClosePeriodAsync`)

Preserve this sequence when modifying period closing:
1. Verify the `(year, month)` period is not already closed (`DomainException`).
2. Verify the target equity account (`equityAccountId`) exists (`NotFoundException`).
3. Fetch all entries for the month via `GetByDateRangeWithLinesAsync(startDate, endDate, includeUnposted: true)` and post (`IsPosted = true`) all unposted entries.
4. Compute the period Income Statement (`GetIncomeStatementAsync(startDate, endDate)`). If there is no activity and `NetIncome == 0`, mark the period closed with no closing entry.
5. Build the closing journal entry (`IsPosted = true`, code `CIE-{year}-{month:D2}`, date `endDate`):
   - Debit every `Revenue` account line.
   - Credit every `Cost` and `Expense` account line.
   - Post the difference (`NetIncome`) to `equityAccountId`: credit if `NetIncome > 0` (profit), debit `Math.Abs(NetIncome)` if `NetIncome < 0` (loss).
   - Validate `closingEntry.ValidateDoubleEntry()` before persisting.
6. Mark/create `AccountingPeriod` as closed (`IsClosed = true`, `ClosedAt`, `ClosingJournalEntryId`) and record `TransactionLog`.

## 4. Provisional Reports & Exports (JSON, Excel, PDF)

- **Dual report controllers**:
  - `ReportsController` (`/api/reports/income-statement`, `/api/reports/balance-sheet`) → JSON wrapped in `SbcGenericResponse<T>`.
  - `FinancialReportsController` (`/api/financialReports/.../excel`, `.../pdf`) → binary `File(...)` downloads with MIME types `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` and `application/pdf`.
- **`includeUnposted` flag**:
  - Propagate `bool includeUnposted = false` down to `IJournalEntryRepository.GetByDateRangeWithLinesAsync(..., includeUnposted)`.
  - When `includeUnposted == true`, set `IsProvisional = true` on the DTO and display `(Provisional)` / `PROVISIONAL` labels in Excel and PDF outputs.
  - `DashboardService.GetSummaryAsync` always uses `includeUnposted = true` for a live accounting snapshot.
- **QuestPDF**: Keep `QuestPDF.Settings.License = LicenseType.Community;` initialized in `FinancialReportService`'s static constructor (or equivalent startup initializer).

## 5. Excel Bulk Import (`BulkImportService` + ClosedXML)

Keep the columns parsed in `ImportFromExcelAsync` strictly synchronized with `GenerateBulkImportTemplateAsync`:

| Column | Index | Content |
| :---: | :---: | :--- |
| A | `1` | Date (`DateTime`) |
| B | `2` | Description (`string`) |
| C | `3` | Account Code (`string`, resolved via `IAccountRepository.GetByCodeAsync`) |
| D | `4` | Debit (`decimal`) |
| E | `5` | Credit (`decimal`) |
| F | `6` | Optional Group Reference (`string`; defaults to `{date:yyyyMMdd}-{description}` when blank) |

- Always persist the summary row in `accounting.BulkImports` and audit in `TransactionLog` with:
  - `TransactionStatus.Success` when `ErrorCount == 0`
  - `TransactionStatus.PartialSuccess` when `ErrorCount > 0 && SuccessCount > 0`
  - `TransactionStatus.Failure` when `SuccessCount == 0 && ErrorCount > 0`

## 6. Verification & Agent Documentation Maintenance

1. Run targeted accounting tests after any change:
   ```bash
   dotnet test SBC.UnitTest/SBC.UnitTest.csproj --filter "FullyQualifiedName~JournalEntry|FullyQualifiedName~AccountingPeriod|FullyQualifiedName~FinancialReport|FullyQualifiedName~BulkImport"
   ```
2. If accounting formulas, Excel column definitions, entry code formats, or closing/reporting flows change, update `AGENTS.md`, this skill (`sbc-accounting-operations`), and their mirrors in `docs/agent-guidelines-en/`.
