using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;

namespace SBC.Application.Services.Interfaces;

public interface IJournalEntryLineService
{
    Task<JournalEntryLineDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<JournalEntryLineDto>> GetByJournalEntryIdAsync(Guid journalEntryId);
    Task<PagedResultDto<JournalEntryLineDto>> GetPagedAsync(JournalEntryLineFilterDto filter);
    Task<JournalEntryLineDto> CreateAsync(CreateJournalEntryLineForLineDto createDto);
    Task UpdateAsync(Guid id, UpdateJournalEntryLineForLineDto updateDto);
    Task DeleteAsync(Guid id);
}
