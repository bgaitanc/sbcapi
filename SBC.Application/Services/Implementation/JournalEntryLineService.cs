using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.Application.Services.Implementation;

public class JournalEntryLineService(
    IJournalEntryLineRepository repository, 
    IJournalEntryRepository journalEntryRepository) : IJournalEntryLineService
{
    public async Task<JournalEntryLineDto?> GetByIdAsync(Guid id)
    {
        var line = await repository.GetByIdAsync(id);
        return line == null ? null : MapToDto(line);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetByJournalEntryIdAsync(Guid journalEntryId)
    {
        var lines = await repository.GetByJournalEntryIdAsync(journalEntryId);
        return lines.Select(MapToDto);
    }

    public async Task<JournalEntryLineDto> CreateAsync(CreateJournalEntryLineForLineDto createDto)
    {
        var entry = await journalEntryRepository.GetByIdWithLinesAsync(createDto.JournalEntryId);
        if (entry == null) throw new NotFoundException(nameof(JournalEntry), createDto.JournalEntryId);
        if (entry.IsPosted) throw new DomainException("No se pueden agregar líneas a un asiento mayorizado.");

        var line = new JournalEntryLine
        {
            Id = Guid.NewGuid(),
            JournalEntryId = createDto.JournalEntryId,
            AccountId = createDto.AccountId,
            Debit = createDto.Debit,
            Credit = createDto.Credit
        };

        // Simular adición para validar partida doble
        entry.Lines.Add(line);
        if (!entry.ValidateDoubleEntry())
        {
            throw new DomainException("La operación resultaría en un asiento que no cumple con la partida doble.");
        }

        var createdLine = await repository.AddAsync(line);
        return MapToDto(createdLine);
    }

    public async Task UpdateAsync(Guid id, UpdateJournalEntryLineForLineDto updateDto)
    {
        var line = await repository.GetByIdWithJournalEntryAsync(id);
        if (line == null) throw new NotFoundException(nameof(JournalEntryLine), id);
        
        var entry = line.JournalEntry;
        if (entry.IsPosted) throw new DomainException("No se puede editar una línea de un asiento mayorizado.");

        line.AccountId = updateDto.AccountId;
        line.Debit = updateDto.Debit;
        line.Credit = updateDto.Credit;

        if (!entry.ValidateDoubleEntry())
        {
            throw new DomainException("La operación resultaría en un asiento que no cumple con la partida doble.");
        }

        await repository.UpdateAsync(line);
    }

    public async Task DeleteAsync(Guid id)
    {
        var line = await repository.GetByIdWithJournalEntryAsync(id);
        if (line == null) throw new NotFoundException(nameof(JournalEntryLine), id);

        var entry = line.JournalEntry;
        if (entry.IsPosted) throw new DomainException("No se puede eliminar una línea de un asiento mayorizado.");

        // Simular eliminación para validar partida doble
        entry.Lines.Remove(line);
        if (entry.Lines.Count > 0 && !entry.ValidateDoubleEntry())
        {
            throw new DomainException("La operación resultaría en un asiento que no cumple con la partida doble.");
        }

        await repository.DeleteAsync(line);
    }

    private static JournalEntryLineDto MapToDto(JournalEntryLine line)
    {
        return new JournalEntryLineDto
        {
            Id = line.Id,
            AccountId = line.AccountId,
            AccountName = line.Account?.Name,
            AccountCode = line.Account?.Code,
            Debit = line.Debit,
            Credit = line.Credit
        };
    }
}
