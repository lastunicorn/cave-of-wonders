using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportInflationTests;

public class ImportSourceWeb_ResponseTests
{
    private readonly ImportInflationUseCase useCase;
    private readonly Mock<IIns> ins;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<IInflationRecordRepository> inflationRecordRepository;

    public ImportSourceWeb_ResponseTests()
    {
        ins = new Mock<IIns>();
        unitOfWork = new Mock<IUnitOfWork>();
        inflationRecordRepository = new Mock<IInflationRecordRepository>();

        unitOfWork
            .SetupGet(x => x.InflationRecordRepository)
            .Returns(inflationRecordRepository.Object);

        useCase = new ImportInflationUseCase(ins.Object, unitOfWork.Object);
    }

    [Fact]
    public async Task HavingInsReturnsOneRecords_AndRepositoryAddsOneAsNew_WhenImportInflations_ThenResponseHasOneAddedOneTotal()
    {
        // Arrange
        List<InflationRecordDto> insRecords =
        [
            new InflationRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .ReturnsAsync(AddOrUpdateResult.Added);

        // Act
        ImportInflationResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(1);
        response.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryAddsTwoAsNew_WhenImportInflations_ThenResponseHasTwoAddedTwoTotal()
    {
        // Arrange
        List<InflationRecordDto> insRecords =
        [
            new InflationRecordDto(),
            new InflationRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .ReturnsAsync(AddOrUpdateResult.Added)
            .ReturnsAsync(AddOrUpdateResult.Added);

        // Act
        ImportInflationResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(2);
        response.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryAddsOneAsNewAndUpdatesOne_WhenImportInflations_ThenResponseHasOneAddedOneUpdatedTwoTotal()
    {
        // Arrange
        List<InflationRecordDto> insRecords =
        [
            new InflationRecordDto(),
            new InflationRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .ReturnsAsync(AddOrUpdateResult.Added)
            .ReturnsAsync(AddOrUpdateResult.Updated);

        // Act
        ImportInflationResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.AddedCount.Should().Be(1);
        response.UpdatedCount.Should().Be(1);
        response.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task HavingInsReturnsTwoRecords_AndRepositoryUpdatesTwo_WhenImportInflations_ThenResponseHasTwoUpdatedTwoTotal()
    {
        // Arrange
        List<InflationRecordDto> insRecords =
        [
            new InflationRecordDto(),
            new InflationRecordDto()
        ];

        inflationRecordRepository
            .SetupSequence(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .ReturnsAsync(AddOrUpdateResult.Updated)
            .ReturnsAsync(AddOrUpdateResult.Updated);

        // Act
        ImportInflationResponse response = await ExecuteUseCase(insRecords);

        // Assert
        response.UpdatedCount.Should().Be(2);
        response.TotalCount.Should().Be(2);
    }

    private async Task<ImportInflationResponse> ExecuteUseCase(List<InflationRecordDto> insRecords)
    {
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(insRecords);

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        return await useCase.Handle(request, CancellationToken.None);
    }
}
