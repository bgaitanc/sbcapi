using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SBC.Api.Controllers;
using SBC.Api.Controllers.Base;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;

namespace SBC.UnitTest.Controllers;

public class JournalEntryLinesControllerTests
{
    private readonly Mock<IJournalEntryLineService> _serviceMock = new();
    private readonly JournalEntryLinesController _controller;

    public JournalEntryLinesControllerTests()
    {
        _controller = new JournalEntryLinesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var line = new JournalEntryLineDto { Id = id, Debit = 100 };
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(line);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<JournalEntryLineDto>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<JournalEntryLineDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(line, response.Data);
    }

    [Fact]
    public async Task GetByJournalEntryId_ShouldReturnOk()
    {
        // Arrange
        var journalEntryId = Guid.NewGuid();
        var lines = new List<JournalEntryLineDto> { new() { Id = Guid.NewGuid(), Debit = 100 } };
        _serviceMock.Setup(s => s.GetByJournalEntryIdAsync(journalEntryId)).ReturnsAsync(lines);

        // Act
        var result = await _controller.GetByJournalEntryId(journalEntryId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<JournalEntryLineDto>>>(result);
        var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<IEnumerable<JournalEntryLineDto>>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(lines, response.Data);
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateJournalEntryLineForLineDto { JournalEntryId = Guid.NewGuid(), AccountId = Guid.NewGuid(), Debit = 100 };
        var createdLine = new JournalEntryLineDto { Id = Guid.NewGuid(), Debit = 100 };
        _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdLine);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<JournalEntryLineDto>>(result);
        var objectResult = Assert.IsType<CreatedResult>(actionResult.Result);
        var response = Assert.IsType<SbcGenericResponse<JournalEntryLineDto>>(objectResult.Value);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateJournalEntryLineForLineDto { AccountId = Guid.NewGuid(), Debit = 200 };
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
