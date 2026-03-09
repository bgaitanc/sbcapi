using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;

namespace SBC.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JournalEntryLinesController(IJournalEntryLineService service) : SbcControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JournalEntryLineDto>> GetById(Guid id)
    {
        return await ExecuteServiceAsync(async () =>
        {
            var line = await service.GetByIdAsync(id);
            if (line == null) throw new NotFoundException(nameof(JournalEntryLine), id);
            return line;
        });
    }

    [HttpGet("journal-entry/{journalEntryId:guid}")]
    public async Task<ActionResult<IEnumerable<JournalEntryLineDto>>> GetByJournalEntryId(Guid journalEntryId)
    {
        return await ExecuteServiceAsync(() => service.GetByJournalEntryIdAsync(journalEntryId));
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntryLineDto>> Create([FromBody] CreateJournalEntryLineForLineDto createDto)
    {
        return await ExecuteServiceAsync(() => service.CreateAsync(createDto), HttpStatusCode.Created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Update(Guid id, [FromBody] UpdateJournalEntryLineForLineDto updateDto)
    {
        return await ExecuteServiceAsync(() => service.UpdateAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Delete(Guid id)
    {
        return await ExecuteServiceAsync(() => service.DeleteAsync(id));
    }
}
