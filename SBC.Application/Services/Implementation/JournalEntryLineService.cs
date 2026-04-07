using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class JournalEntryLineService(
    IJournalEntryLineRepository repository, 
    IJournalEntryRepository journalEntryRepository,
    ITransactionLogService transactionLogService) : IJournalEntryLineService
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
        if (entry == null)
        {
            var error = $"JournalEntry with id {createDto.JournalEntryId} was not found.";
            await transactionLogService.LogTransactionAsync(null, "CreateJournalEntryLine", "JournalEntryLine", null, "Failure", JsonSerializer.Serialize(createDto), error);
            throw new NotFoundException(nameof(JournalEntry), createDto.JournalEntryId);
        }
        if (entry.IsPosted)
        {
            var error = "No se pueden agregar líneas a un asiento mayorizado.";
            await transactionLogService.LogTransactionAsync(null, "CreateJournalEntryLine", "JournalEntryLine", null, "ValidationError", JsonSerializer.Serialize(createDto), error);
            throw new DomainException(error);
        }

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
            var error = "La operación resultaría en un asiento que no cumple con la partida doble.";
            await transactionLogService.LogTransactionAsync(null, "CreateJournalEntryLine", "JournalEntryLine", null, "ValidationError", JsonSerializer.Serialize(createDto), error);
            throw new DomainException(error);
        }

        var createdLine = await repository.AddAsync(line);
        await transactionLogService.LogTransactionAsync(null, "CreateJournalEntryLine", "JournalEntryLine", createdLine.Id.ToString(), "Success", JsonSerializer.Serialize(createDto));
        return MapToDto(createdLine);
    }

    public async Task UpdateAsync(Guid id, UpdateJournalEntryLineForLineDto updateDto)
    {
        var line = await repository.GetByIdWithJournalEntryAsync(id);
        if (line == null)
        {
            var error = $"JournalEntryLine with id {id} was not found.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntryLine", "JournalEntryLine", id.ToString(), "Failure", JsonSerializer.Serialize(updateDto), error);
            throw new NotFoundException(nameof(JournalEntryLine), id);
        }
        
        var entry = line.JournalEntry;
        if (entry.IsPosted)
        {
            var error = "No se puede editar una línea de un asiento mayorizado.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntryLine", "JournalEntryLine", id.ToString(), "ValidationError", JsonSerializer.Serialize(updateDto), error);
            throw new DomainException(error);
        }

        line.AccountId = updateDto.AccountId;
        line.Debit = updateDto.Debit;
        line.Credit = updateDto.Credit;

        if (!entry.ValidateDoubleEntry())
        {
            var error = "La operación resultaría en un asiento que no cumple con la partida doble.";
            await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntryLine", "JournalEntryLine", id.ToString(), "ValidationError", JsonSerializer.Serialize(updateDto), error);
            throw new DomainException(error);
        }

        await repository.UpdateAsync(line);
        await transactionLogService.LogTransactionAsync(null, "UpdateJournalEntryLine", "JournalEntryLine", id.ToString(), "Success", JsonSerializer.Serialize(updateDto));
    }

    public async Task DeleteAsync(Guid id)
    {
        var line = await repository.GetByIdWithJournalEntryAsync(id);
        if (line == null)
        {
            var error = $"JournalEntryLine with id {id} was not found.";
            await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntryLine", "JournalEntryLine", id.ToString(), "Failure", null, error);
            throw new NotFoundException(nameof(JournalEntryLine), id);
        }

        var entry = line.JournalEntry;
        if (entry.IsPosted)
        {
            var error = "No se puede eliminar una línea de un asiento mayorizado.";
            await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntryLine", "JournalEntryLine", id.ToString(), "ValidationError", null, error);
            throw new DomainException(error);
        }

        // Simular eliminación para validar partida doble
        entry.Lines.Remove(line);
        if (entry.Lines.Count > 0 && !entry.ValidateDoubleEntry())
        {
            var error = "La operación resultaría en un asiento que no cumple con la partida doble.";
            await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntryLine", "JournalEntryLine", id.ToString(), "ValidationError", null, error);
            throw new DomainException(error);
        }

        await repository.DeleteAsync(line);
        await transactionLogService.LogTransactionAsync(null, "DeleteJournalEntryLine", "JournalEntryLine", id.ToString(), "Success");
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
