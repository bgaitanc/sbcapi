using Moq;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest.Services;

public class JournalEntryServiceTests
{
    private readonly Mock<IJournalEntryRepository> _repositoryMock = new();
    private readonly JournalEntryService _service;

    public JournalEntryServiceTests()
    {
        _service = new JournalEntryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenEntryExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entry = new JournalEntry { Id = id, Date = DateTime.UtcNow, Description = "Test Entry" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entry);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entry.Id, result.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenValid()
    {
        // Arrange
        var createDto = new CreateJournalEntryDto
        {
            Date = DateTime.UtcNow,
            Description = "Test Entry",
            Lines = new List<CreateJournalEntryLineDto>
            {
                new() { AccountId = Guid.NewGuid(), Debit = 100, Credit = 0 },
                new() { AccountId = Guid.NewGuid(), Debit = 0, Credit = 100 }
            }
        };

        _repositoryMock.Setup(r => r.GetLastCodeAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync("");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<JournalEntry>()))
            .ReturnsAsync((JournalEntry e) => e);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Lines.Count);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<JournalEntry>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenDoubleEntryFails()
    {
        // Arrange
        var createDto = new CreateJournalEntryDto
        {
            Date = DateTime.UtcNow,
            Description = "Invalid Entry",
            Lines = new List<CreateJournalEntryLineDto>
            {
                new() { AccountId = Guid.NewGuid(), Debit = 100, Credit = 0 },
                new() { AccountId = Guid.NewGuid(), Debit = 0, Credit = 50 } // No balancea
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenEntryIsPosted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateJournalEntryDto { Date = DateTime.UtcNow, Description = "Update", Lines = new List<UpdateJournalEntryLineDto>() };
        var existingEntry = new JournalEntry { Id = id, IsPosted = true };
        _repositoryMock.Setup(r => r.GetByIdWithLinesAsync(id)).ReturnsAsync(existingEntry);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.UpdateAsync(id, updateDto));
        Assert.Equal("No se puede editar un asiento que ya ha sido mayorizado.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenEntryIsPosted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingEntry = new JournalEntry { Id = id, IsPosted = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntry);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.DeleteAsync(id));
        Assert.Equal("No se puede eliminar un asiento que ya ha sido mayorizado.", exception.Message);
    }
}
