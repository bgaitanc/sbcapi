using SBC.Application.Models.Accounting;

namespace SBC.Application.Services.Interfaces;

public interface IJournalEntryLineService
{
    Task<JournalEntryLineDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<JournalEntryLineDto>> GetByJournalEntryIdAsync(Guid journalEntryId);
    Task<JournalEntryLineDto> CreateAsync(CreateJournalEntryLineForLineDto createDto);
    Task UpdateAsync(Guid id, UpdateJournalEntryLineForLineDto updateDto);
    Task DeleteAsync(Guid id);
}
