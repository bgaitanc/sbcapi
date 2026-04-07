using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;

namespace SBC.Application.Services.Interfaces;

public interface IJournalEntryService
{
    Task<JournalEntryDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<JournalEntryDto>> GetAllAsync();
    Task<PagedResultDto<JournalEntryDto>> GetPagedAsync(JournalEntryFilterDto filter);
    Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto createDto);
    Task UpdateAsync(Guid id, UpdateJournalEntryDto updateDto);
    Task DeleteAsync(Guid id);
}
