using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;
using ClosedXML.Excel;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class JournalEntryService(IJournalEntryRepository repository, ITransactionLogService transactionLogService) : IJournalEntryService
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
            var error = "El asiento no cumple con el principio de partida doble.";
            await transactionLogService.LogTransactionAsync(null, "CreateJournalEntry", "JournalEntry", null, "ValidationError", JsonSerializer.Serialize(createDto), error);
            throw new DomainException(error);
        }

        entry.Code = await GenerateCodeAsync(entry.Year, entry.Month);

        var createdEntry = await repository.AddAsync(entry);
        await transactionLogService.LogTransactionAsync(null, "CreateJournalEntry", "JournalEntry", createdEntry.Id.ToString(), "Success", JsonSerializer.Serialize(createDto));
        return MapToDto(createdEntry);
    }

    public async Task UpdateAsync(Guid id, UpdateJournalEntryDto updateDto)
    {
        var entry = await repository.GetByIdWithLinesAsync(id);
        if (entry == null)
        {
            var error = $"JournalEntry with id {id} was not found.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntry", "JournalEntry", id.ToString(), "Failure", JsonSerializer.Serialize(updateDto), error);
            throw new NotFoundException(nameof(JournalEntry), id);
        }
        if (entry.IsPosted)
        {
            var error = "No se puede editar un asiento que ya ha sido mayorizado.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntry", "JournalEntry", id.ToString(), "ValidationError", JsonSerializer.Serialize(updateDto), error);
            throw new DomainException(error);
        }

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
            var error = "El asiento no cumple con el principio de partida doble.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntry", "JournalEntry", id.ToString(), "ValidationError", JsonSerializer.Serialize(updateDto), error);
            throw new DomainException(error);
        }

        await repository.UpdateAsync(entry);
        await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntry", "JournalEntry", id.ToString(), "Success", JsonSerializer.Serialize(updateDto));
    }

    public async Task DeleteAsync(Guid id)
    {
        var entry = await repository.GetByIdAsync(id);
        if (entry == null)
        {
            var error = $"JournalEntry with id {id} was not found.";
            await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntry", "JournalEntry", id.ToString(), "Failure", null, error);
            throw new NotFoundException(nameof(JournalEntry), id);
        }
        if (entry.IsPosted)
        {
            var error = "No se puede eliminar un asiento que ya ha sido mayorizado.";
            await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntry", "JournalEntry", id.ToString(), "ValidationError", null, error);
            throw new DomainException(error);
        }

        await repository.DeleteAsync(entry);
        await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntry", "JournalEntry", id.ToString(), "Success");
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
