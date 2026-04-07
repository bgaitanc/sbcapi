using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Models.Common;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class JournalEntriesControllerTests
{
    private readonly Mock<IJournalEntryService> _serviceMock = new();
    private readonly JournalEntriesController _controller;

    public JournalEntriesControllerTests()
    {
        _controller = new JournalEntriesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetPaged_ShouldReturnOk()
    {
        // Arrange
        var filter = new JournalEntryFilterDto { PageNumber = 1, PageSize = 10 };
        var pagedResult = new PagedResultDto<JournalEntryDto> 
        { 
            Items = new List<JournalEntryDto> { new() { Id = Guid.NewGuid(), Description = "Test Entry" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _serviceMock.Setup(s => s.GetPagedAsync(filter)).ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetPaged(filter);

        // Assert
        var actionResult = Assert.IsType<ActionResult<PagedResultDto<JournalEntryDto>>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<PagedResultDto<JournalEntryDto>>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(pagedResult, response.Data);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entry = new JournalEntryDto { Id = id, Description = "Test Entry" };
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(entry);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<JournalEntryDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<JournalEntryDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(entry, response.Data);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateJournalEntryDto { Description = "New Entry", Date = DateTime.UtcNow };
        var createdEntry = new JournalEntryDto { Id = Guid.NewGuid(), Description = createDto.Description };
        _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdEntry);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<JournalEntryDto>>(result);
        var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<JournalEntryDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateJournalEntryDto { Description = "Updated Entry", Date = DateTime.UtcNow };
        _serviceMock.Setup(s => s.UpdateAsync(id, updateDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, updateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
        Assert.IsType<NoContentResult>(actionResult.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<SbcGenericResponse>>(result);
        Assert.IsType<NoContentResult>(actionResult.Result);
    }
}
