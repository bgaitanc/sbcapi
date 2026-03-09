using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.Application.Services.Implementation;

public class JournalEntryService(IJournalEntryRepository repository) : IJournalEntryService
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
