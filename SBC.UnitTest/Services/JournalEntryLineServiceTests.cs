using Moq;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest.Services;

public class JournalEntryLineServiceTests
{
    private readonly Mock<IJournalEntryLineRepository> _repositoryMock = new();
    private readonly Mock<IJournalEntryRepository> _journalRepoMock = new();
    private readonly JournalEntryLineService _service;

    public JournalEntryLineServiceTests()
    {
        _service = new JournalEntryLineService(_repositoryMock.Object, _journalRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenEntryIsPosted()
    {
        // Arrange
        var createDto = new CreateJournalEntryLineForLineDto { JournalEntryId = Guid.NewGuid(), AccountId = Guid.NewGuid(), Debit = 100 };
        var entry = new JournalEntry { Id = createDto.JournalEntryId, IsPosted = true };
        _journalRepoMock.Setup(r => r.GetByIdWithLinesAsync(createDto.JournalEntryId)).ReturnsAsync(entry);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(createDto));
        Assert.Equal("No se pueden agregar líneas a un asiento mayorizado.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenDoubleEntryFails()
    {
        // Arrange
        var createDto = new CreateJournalEntryLineForLineDto { JournalEntryId = Guid.NewGuid(), AccountId = Guid.NewGuid(), Debit = 100 };
        var entry = new JournalEntry 
        { 
            Id = createDto.JournalEntryId, 
            IsPosted = false,
            Lines = new List<JournalEntryLine> { new() { Debit = 50, Credit = 0 } }
        };
        _journalRepoMock.Setup(r => r.GetByIdWithLinesAsync(createDto.JournalEntryId)).ReturnsAsync(entry);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(createDto));
        Assert.Equal("La operación resultaría en un asiento que no cumple con la partida doble.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenEntryIsPosted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateJournalEntryLineForLineDto { AccountId = Guid.NewGuid(), Debit = 200 };
        var line = new JournalEntryLine { Id = id, JournalEntry = new JournalEntry { IsPosted = true } };
        _repositoryMock.Setup(r => r.GetByIdWithJournalEntryAsync(id)).ReturnsAsync(line);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.UpdateAsync(id, updateDto));
        Assert.Equal("No se puede editar una línea de un asiento mayorizado.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenEntryIsPosted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var line = new JournalEntryLine { Id = id, JournalEntry = new JournalEntry { IsPosted = true } };
        _repositoryMock.Setup(r => r.GetByIdWithJournalEntryAsync(id)).ReturnsAsync(line);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.DeleteAsync(id));
        Assert.Equal("No se puede eliminar una línea de un asiento mayorizado.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entry = new JournalEntry { IsPosted = false };
        var line1 = new JournalEntryLine { Id = id, JournalEntry = entry, Debit = 100, Credit = 0 };
        var line2 = new JournalEntryLine { Id = Guid.NewGuid(), JournalEntry = entry, Debit = 0, Credit = 100 };
        entry.Lines = new List<JournalEntryLine> { line1, line2 };
        
        _repositoryMock.Setup(r => r.GetByIdWithJournalEntryAsync(id)).ReturnsAsync(line1);

        // Act
        // Borrar line1 hará que no balancee (solo queda line2 con Credit=100)
        // Pero espera, si borro line1, queda line2. Sum(Debit)=0, Sum(Credit)=100. No balancea.
        // El código dice: entry.Lines.Remove(line); if (entry.Lines.Count > 0 && !entry.ValidateDoubleEntry()) throw ...
        
        var exception = await Assert.ThrowsAsync<DomainException>(() => _service.DeleteAsync(id));
        Assert.Equal("La operación resultaría en un asiento que no cumple con la partida doble.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenEntryBecomesEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entry = new JournalEntry { IsPosted = false };
        var line = new JournalEntryLine { Id = id, JournalEntry = entry, Debit = 100, Credit = 100 }; // Asiento de una sola línea que balancea (raro pero posible en el modelo)
        entry.Lines = new List<JournalEntryLine> { line };
        
        _repositoryMock.Setup(r => r.GetByIdWithJournalEntryAsync(id)).ReturnsAsync(line);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(line), Times.Once);
    }
}
