using Moq;
using SBC.Application.Services.Implementation;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Repositories.Interfaces;

namespace SBC.UnitTest.Services;

public class BulkImportServiceTests
{
    private readonly Mock<IBulkImportRepository> _repositoryMock = new();
    private readonly Mock<IJournalEntryRepository> _journalEntryRepositoryMock = new();
    private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
    private readonly BulkImportService _service;

    public BulkImportServiceTests()
    {
        _service = new BulkImportService(
            _repositoryMock.Object, 
            _journalEntryRepositoryMock.Object, 
            _accountRepositoryMock.Object);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnDtos()
    {
        // Arrange
        var imports = new List<BulkImport>
        {
            new() { Id = Guid.NewGuid(), FileName = "test1.xlsx", SuccessCount = 10, CreatedAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), FileName = "test2.xlsx", SuccessCount = 5, CreatedAt = DateTime.Now.AddHours(-1) }
        };
        _repositoryMock.Setup(r => r.GetAllNoTrackingAsync()).ReturnsAsync(imports);

        // Act
        var result = await _service.GetHistoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("test1.xlsx", result.First().FileName);
    }
}
