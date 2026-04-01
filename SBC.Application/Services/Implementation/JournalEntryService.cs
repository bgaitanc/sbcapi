using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;
using ClosedXML.Excel;

namespace SBC.Application.Services.Implementation;

public class JournalEntryService(IJournalEntryRepository repository, IAccountRepository accountRepository) : IJournalEntryService
{
    public async Task<JournalEntryDto?> GetByIdAsync(Guid id)
    {
        var entry = await repository.GetByIdAsync(id);
        return entry == null ? null : MapToDto(entry);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetAllAsync()
    {
        var entries = await repository.GetAllAsync();
        return entries.Select(MapToDto);
    }

    public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto createDto)
    {
        var entry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            Date = createDto.Date,
            Day = createDto.Date.Day,
            Month = createDto.Date.Month,
            Year = createDto.Date.Year,
            Description = createDto.Description,
            Lines = createDto.Lines.Select(l => new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = l.AccountId,
                Debit = l.Debit,
                Credit = l.Credit
            }).ToList()
        };

        if (!entry.ValidateDoubleEntry())
        {
            throw new DomainException("El asiento no cumple con el principio de partida doble.");
        }

        entry.Code = await GenerateCodeAsync(entry.Year, entry.Month);

        var createdEntry = await repository.AddAsync(entry);
        return MapToDto(createdEntry);
    }

    public async Task UpdateAsync(Guid id, UpdateJournalEntryDto updateDto)
    {
        var entry = await repository.GetByIdWithLinesAsync(id);
        if (entry == null) throw new NotFoundException(nameof(JournalEntry), id);
        if (entry.IsPosted) throw new DomainException("No se puede editar un asiento que ya ha sido mayorizado.");

        entry.Date = updateDto.Date;
        entry.Day = updateDto.Date.Day;
        entry.Month = updateDto.Date.Month;
        entry.Year = updateDto.Date.Year;
        entry.Description = updateDto.Description;

        // Actualizar líneas
        entry.Lines.Clear();
        foreach (var lineDto in updateDto.Lines)
        {
            entry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = id,
                AccountId = lineDto.AccountId,
                Debit = lineDto.Debit,
                Credit = lineDto.Credit
            });
        }

        if (!entry.ValidateDoubleEntry())
        {
            throw new DomainException("El asiento no cumple con el principio de partida doble.");
        }

        await repository.UpdateAsync(entry);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entry = await repository.GetByIdAsync(id);
        if (entry == null) throw new NotFoundException(nameof(JournalEntry), id);
        if (entry.IsPosted) throw new DomainException("No se puede eliminar un asiento que ya ha sido mayorizado.");

        await repository.DeleteAsync(entry);
    }

    public async Task<BulkJournalEntryImportResultDto> ImportFromExcelAsync(Stream excelStream)
    {
        var result = new BulkJournalEntryImportResultDto();
        using var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            result.Errors.Add("El archivo Excel no contiene ninguna hoja.");
            result.ErrorCount = 1;
            return result;
        }

        // Formato esperado:
        // Col A: Fecha (DateTime)
        // Col B: Descripción (String)
        // Col C: Código Cuenta (String)
        // Col D: Debe (Decimal)
        // Col E: Haber (Decimal)
        // Col F: Referencia Agrupador (Opcional, para agrupar líneas en un mismo asiento si la fecha y descripción coinciden)

        var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezado
        var journalEntriesToCreate = new List<JournalEntry>();
        var currentGroup = string.Empty;
        JournalEntry? currentEntry = null;

        int rowNumber = 1;
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

                // Lógica de agrupación: si groupRef es igual al anterior y no está vacío, pertenece al mismo asiento
                // Si es vacío, cada línea es un asiento (no recomendado pero posible) o usamos fecha+descripción como clave
                var currentKey = string.IsNullOrWhiteSpace(groupRef) 
                    ? $"{date:yyyyMMdd}-{description}" 
                    : groupRef;

                if (currentEntry == null || currentGroup != currentKey)
                {
                    // Si ya teníamos un asiento, validamos partida doble antes de empezar uno nuevo
                    if (currentEntry != null)
                    {
                        if (!currentEntry.ValidateDoubleEntry())
                        {
                            result.Errors.Add($"Asiento '{currentEntry.Description}' ({currentGroup}): No cumple partida doble (Debe: {currentEntry.Lines.Sum(l => l.Debit)}, Haber: {currentEntry.Lines.Sum(l => l.Credit)}).");
                            // No añadimos este asiento a la lista final
                        }
                        else
                        {
                            currentEntry.Code = await GenerateCodeAsync(currentEntry.Year, currentEntry.Month);
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

        // Validar el último asiento procesado
        if (currentEntry != null)
        {
            if (!currentEntry.ValidateDoubleEntry())
            {
                result.Errors.Add($"Asiento '{currentEntry.Description}' ({currentGroup}): No cumple partida doble (Debe: {currentEntry.Lines.Sum(l => l.Debit)}, Haber: {currentEntry.Lines.Sum(l => l.Credit)}).");
            }
            else
            {
                currentEntry.Code = await GenerateCodeAsync(currentEntry.Year, currentEntry.Month);
                journalEntriesToCreate.Add(currentEntry);
                result.SuccessCount++;
            }
        }

        // Guardar todos los asientos válidos
        foreach (var entry in journalEntriesToCreate)
        {
            await repository.AddAsync(entry);
        }

        result.ErrorCount = result.Errors.Count;
        return result;
    }

    private async Task<string> GenerateCodeAsync(int year, int month)
    {
        var lastCode = await repository.GetLastCodeAsync(year, month);
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

    private static JournalEntryDto MapToDto(JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            Code = entry.Code,
            Date = entry.Date,
            Description = entry.Description,
            IsPosted = entry.IsPosted,
            CreatedAt = entry.CreatedAt,
            CreatedBy = entry.CreatedBy,
            UpdatedAt = entry.UpdatedAt,
            UpdatedBy = entry.UpdatedBy,
            Lines = entry.Lines.Select(l => new JournalEntryLineDto
            {
                Id = l.Id,
                AccountId = l.AccountId,
                AccountName = l.Account?.Name,
                AccountCode = l.Account?.Code,
                Debit = l.Debit,
                Credit = l.Credit
            }).ToList()
        };
    }
}
