using ClosedXML.Excel;
using SBC.Application.Models.Accounting;
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
