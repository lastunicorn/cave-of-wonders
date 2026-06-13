using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportCpiUseCaseTests;

public class ImportSourceFile_ResponseTests
{
    private readonly ImportCpiUseCase useCase;
    private readonly Mock<IInsService> ins;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<ICpiRepository> inflationRecordRepository;

    public ImportSourceFile_ResponseTests()
    {
        ins = new Mock<IInsService>();
        unitOfWork = new Mock<IUnitOfWork>();
        inflationRecordRepository = new Mock<ICpiRepository>();

        unitOfWork
            .SetupGet(x => x.CpiRepository)
            .Returns(inflationRecordRepository.Object);

        useCase = new ImportCpiUseCase(ins.Object, unitOfWork.Object, null);
    }

    [Fact]
    public async Task HavingInsReturnsOneRecords_AndRepositoryAddsOneAsNew_WhenImportInflations_ThenResponseHasOneAddedOneTotal()
    {
        // Arrange
        List<CpiRecordDto> insRecords =
        [
            new CpiRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<Cpi>()))
            .ReturnsAsync(AddOrUpdateResult.Added);

        // Act
        ImportCpiResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(1);
        response.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryAddsTwoAsNew_WhenImportInflations_ThenResponseHasTwoAddedTwoTotal()
    {
        // Arrange
        List<CpiRecordDto> insRecords =
        [
            new CpiRecordDto(),
            new CpiRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<Cpi>()))
            .ReturnsAsync(AddOrUpdateResult.Added)
            .ReturnsAsync(AddOrUpdateResult.Added);

        // Act
        ImportCpiResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(2);
        response.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryAddsOneAsNewAndUpdatesOne_WhenImportInflations_ThenResponseHasOneAddedOneUpdatedTwoTotal()
    {
        // Arrange
        List<CpiRecordDto> insRecords =
        [
            new CpiRecordDto(),
            new CpiRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<Cpi>()))
            .ReturnsAsync(AddOrUpdateResult.Added)
            .ReturnsAsync(AddOrUpdateResult.Updated);

        // Act
        ImportCpiResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(1);
        response.UpdatedCount.Should().Be(1);
        response.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryUpdatesTwo_WhenImportInflations_ThenResponseHasTwoUpdatedTwoTotal()
    {
        // Arrange
        List<CpiRecordDto> insRecords =
        [
            new CpiRecordDto(),
            new CpiRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<Cpi>()))
            .ReturnsAsync(AddOrUpdateResult.Updated)
            .ReturnsAsync(AddOrUpdateResult.Updated);

        // Act
        ImportCpiResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.UpdatedCount.Should().Be(2);
        response.TotalCount.Should().Be(2);
    }

    private async Task<ImportCpiResponse> ExecuteUseCase(List<CpiRecordDto> insRecords)
    {
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(insRecords);

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file2"
        };

        return await useCase.Handle(request, CancellationToken.None);
    }
}
