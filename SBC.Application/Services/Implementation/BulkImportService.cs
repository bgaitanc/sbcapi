using ClosedXML.Excel;
using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

/// <summary>
/// Implementation of the bulk import service.
/// </summary>
public class BulkImportService(
    IBulkImportRepository repository,
    IJournalEntryRepository journalEntryRepository,
    IAccountRepository accountRepository,
    IAccountingPeriodRepository accountingPeriodRepository,
    ITransactionLogService transactionLogService) : IBulkImportService
{
    public async Task<BulkJournalEntryImportResultDto> ImportFromExcelAsync(Stream excelStream, string fileName)
    {
        var result = new BulkJournalEntryImportResultDto();
        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            var error = "El archivo Excel no contiene ninguna hoja.";
            result.Errors.Add(error);
            result.ErrorCount = 1;
            await transactionLogService.LogTransactionAsync(null, TransactionActions.BulkImport, nameof(BulkImport), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { fileName }), error);
            return result;
        }

        var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezado
        var journalEntriesToCreate = new List<JournalEntry>();
        var currentGroup = string.Empty;
        JournalEntry? currentEntry = null;

        int rowNumber = 1;
        int attemptedEntriesCount = 0;

        foreach (var row in rows)
        {
            rowNumber++;
            try
            {
                var date = row.Cell(1).GetDateTime();
                var description = row.Cell(2).GetString();
                var accountCode = row.Cell(3).GetString();
                var debit = row.Cell(4).GetValue<decimal>();
                var credit = row.Cell(5).GetValue<decimal>();
                var groupRef = row.Cell(6).GetString();

                if (!await accountingPeriodRepository.IsPeriodOpenAsync(date.Year, date.Month))
                {
                    result.Errors.Add($"Fila {rowNumber}: No existe un periodo contable abierto para {date:MMMM yyyy}.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    result.Errors.Add($"Fila {rowNumber}: El código de cuenta es obligatorio.");
                    continue;
                }

                var account = await accountRepository.GetByCodeAsync(accountCode);
                if (account == null)
                {
                    result.Errors.Add($"Fila {rowNumber}: No se encontró la cuenta con código '{accountCode}'.");
                    continue;
                }

                var currentKey = string.IsNullOrWhiteSpace(groupRef) 
                    ? $"{date:yyyyMMdd}-{description}" 
                    : groupRef;

                if (currentEntry == null || currentGroup != currentKey)
                {
                    if (currentEntry != null)
                    {
                        attemptedEntriesCount++;
                        if (!currentEntry.ValidateDoubleEntry())
                        {
                            result.Errors.Add($"Asiento '{currentEntry.Description}' ({currentGroup}): No cumple partida doble.");
                        }
                        else
                        {
                            currentEntry.Code = await GenerateCodeAsync(currentEntry.Year, currentEntry.Month, journalEntriesToCreate);
                            journalEntriesToCreate.Add(currentEntry);
                            result.SuccessCount++;
                        }
                    }

                    currentEntry = new JournalEntry
                    {
                        Id = Guid.NewGuid(),
                        Date = date,
                        Day = date.Day,
                        Month = date.Month,
                        Year = date.Year,
                        Description = description,
                        Lines = new List<JournalEntryLine>()
                    };
                    currentGroup = currentKey;
                }

                currentEntry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = currentEntry.Id,
                    AccountId = account.Id,
                    Debit = debit,
                    Credit = credit
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {rowNumber}: Error al procesar la fila. Detalle: {ex.Message}");
            }
        }

        if (currentEntry != null)
        {
            attemptedEntriesCount++;
            if (!currentEntry.ValidateDoubleEntry())
            {
                result.Errors.Add($"Asiento '{currentEntry.Description}' ({currentGroup}): No cumple partida doble.");
            }
            else
            {
                currentEntry.Code = await GenerateCodeAsync(currentEntry.Year, currentEntry.Month, journalEntriesToCreate);
                journalEntriesToCreate.Add(currentEntry);
                result.SuccessCount++;
            }
        }

        foreach (var entry in journalEntriesToCreate)
        {
            await journalEntryRepository.AddAsync(entry);
        }

        result.ErrorCount = result.Errors.Count;

        // Registrar la carga masiva
        var bulkImport = new BulkImport
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            SuccessCount = result.SuccessCount,
            ErrorCount = result.ErrorCount,
            TotalCount = attemptedEntriesCount
        };
        await repository.AddAsync(bulkImport);

        var status = result.ErrorCount == 0 ? TransactionStatus.Success : (result.SuccessCount > 0 ? TransactionStatus.PartialSuccess : TransactionStatus.Failure);
        var details = JsonSerializer.Serialize(new { fileName, result.SuccessCount, result.ErrorCount, TotalCount = attemptedEntriesCount });
        var errorMessage = result.ErrorCount > 0 ? string.Join("; ", result.Errors.Take(5)) : null;
        await transactionLogService.LogTransactionAsync(null, TransactionActions.BulkImport, nameof(BulkImport), bulkImport.Id.ToString(), status, details, errorMessage);

        return result;
    }

    public async Task<IEnumerable<BulkImportDto>> GetHistoryAsync()
    {
        var imports = await repository.GetAllNoTrackingAsync();
        return imports.Select(i => new BulkImportDto
        {
            Id = i.Id,
            FileName = i.FileName,
            SuccessCount = i.SuccessCount,
            ErrorCount = i.ErrorCount,
            TotalCount = i.TotalCount,
            CreatedAt = i.CreatedAt,
            CreatedBy = i.CreatedBy
        }).OrderByDescending(i => i.CreatedAt);
    }

    public async Task<PagedResultDto<BulkImportDto>> GetPagedHistoryAsync(BulkImportFilterDto filter)
    {
        var (items, totalCount) = await repository.GetPagedAsync(
            filter.FileName, filter.FromDate, filter.ToDate, filter.PageNumber, filter.PageSize);

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GetBulkImports, nameof(BulkImport), null, TransactionStatus.Success, JsonSerializer.Serialize(filter));

        return new PagedResultDto<BulkImportDto>
        {
            Items = items.Select(i => new BulkImportDto
            {
                Id = i.Id,
                FileName = i.FileName,
                SuccessCount = i.SuccessCount,
                ErrorCount = i.ErrorCount,
                TotalCount = i.TotalCount,
                CreatedAt = i.CreatedAt,
                CreatedBy = i.CreatedBy
            }),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<byte[]> GenerateBulkImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Plantilla Carga Masiva");

        // Encabezados
        worksheet.Cell(1, 1).Value = "Fecha";
        worksheet.Cell(1, 2).Value = "Descripción";
        worksheet.Cell(1, 3).Value = "Código de Cuenta";
        worksheet.Cell(1, 4).Value = "Debe";
        worksheet.Cell(1, 5).Value = "Haber";
        worksheet.Cell(1, 6).Value = "Referencia de Grupo (Opcional)";

        // Estilo encabezados
        var headerRange = worksheet.Range(1, 1, 1, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Datos de prueba
        var today = DateTime.Today;
        var testData = new[]
        {
            new { Date = today, Desc = "Apertura de caja inicial", Account = "1.1.1.1.001", Debit = 5000m, Credit = 0m, Group = "AS-001" },
            new { Date = today, Desc = "Apertura de caja inicial", Account = "3.1.1.1.001", Debit = 0m, Credit = 5000m, Group = "AS-001" },
            new { Date = today, Desc = "Pago de servicios de energía", Account = "6.1.4.2.01", Debit = 150m, Credit = 0m, Group = "AS-002" },
            new { Date = today, Desc = "Pago de servicios de energía", Account = "1.1.1.2.001", Debit = 0m, Credit = 150m, Group = "AS-002" }
        };

        for (int i = 0; i < testData.Length; i++)
        {
            worksheet.Cell(i + 2, 1).Value = testData[i].Date;
            worksheet.Cell(i + 2, 2).Value = testData[i].Desc;
            worksheet.Cell(i + 2, 3).Value = testData[i].Account;
            worksheet.Cell(i + 2, 4).Value = testData[i].Debit;
            worksheet.Cell(i + 2, 5).Value = testData[i].Credit;
            worksheet.Cell(i + 2, 6).Value = testData[i].Group;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GetBulkImportTemplate, nameof(BulkImport), null, TransactionStatus.Success, "Generación de plantilla de prueba exitosa.");

        return stream.ToArray();
    }

    private async Task<string> GenerateCodeAsync(int year, int month, List<JournalEntry> currentList)
    {
        var lastCode = await journalEntryRepository.GetLastCodeAsync(year, month);
        
        // Considerar también los que están en la lista actual pero aún no guardados
        var lastInList = currentList
            .Where(e => e.Year == year && e.Month == month)
            .OrderByDescending(e => e.Code)
            .Select(e => e.Code)
            .FirstOrDefault();

        if (string.Compare(lastInList, lastCode) > 0)
        {
            lastCode = lastInList;
        }

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCode))
        {
            var parts = lastCode.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{year}-{month:D2}-{nextNumber:D4}";
    }
}
