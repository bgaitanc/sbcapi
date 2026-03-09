using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

public interface IJournalEntryService
{
    Task<JournalEntryDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<JournalEntryDto>> GetAllAsync();
    Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto createDto);
    Task UpdateAsync(Guid id, UpdateJournalEntryDto updateDto);
    Task DeleteAsync(Guid id);
}
