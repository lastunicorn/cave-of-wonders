using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportInflationTests;

public class ImportSourceWebTests
{
    private readonly ImportInflationUseCase useCase;
    private readonly Mock<IIns> ins;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<IInflationRecordRepository> inflationRecordRepository;

    public ImportSourceWebTests()
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
    public async Task HavingSourceTypeWeb_WhenImportInflations_ThenInsServiceIsCalled()
    {
        // Arrange
        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        ins.Verify(x => x.GetInflationValuesFromWeb(), Times.Once);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsThrows_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .Throws<Exception>();

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InsWebPageException>();
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsOneItem_WhenImportInflations_ThenRepositoryReceivesOneItem()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Once);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsTwoItems_WhenImportInflations_ThenRepositoryReceivesTwoItems()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto(),
                new InflationRecordDto()
            });

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Exactly(2));
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsNoItem_WhenImportInflations_ThenRepositoryIsNotCalled()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<InflationRecordDto>());

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Never);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsItems_AndRepositoryThrows_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        inflationRecordRepository
            .Setup(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .Throws<Exception>();

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DataStorageException>();
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsData_AndRepositoryAcceptsData_WhenImportInflations_ThenUnitOfWorkIsPersisted()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        ImportInflationRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        unitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }
}
