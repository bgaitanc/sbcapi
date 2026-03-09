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
public class JournalEntriesController(IJournalEntryService service) : SbcControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetAll()
    {
        return await ExecuteServiceAsync(() => service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JournalEntryDto>> GetById(Guid id)
    {
        return await ExecuteServiceAsync(async () =>
        {
            var entry = await service.GetByIdAsync(id);
            if (entry == null) throw new NotFoundException(nameof(JournalEntry), id);
            return entry;
        });
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntryDto>> Create([FromBody] CreateJournalEntryDto createDto)
    {
        return await ExecuteServiceAsync(() => service.CreateAsync(createDto), HttpStatusCode.Created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Update(Guid id, [FromBody] UpdateJournalEntryDto updateDto)
    {
        return await ExecuteServiceAsync(() => service.UpdateAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<SbcGenericResponse>> Delete(Guid id)
    {
        return await ExecuteServiceAsync(() => service.DeleteAsync(id));
    }
}
