using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportInflationUseCaseTests;

public class ImportSourceFileTests
{
    private readonly ImportCpiUseCase useCase;
    private readonly Mock<IInsService> ins;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<IInflationRecordRepository> inflationRecordRepository;

    public ImportSourceFileTests()
    {
        ins = new Mock<IInsService>();
        unitOfWork = new Mock<IUnitOfWork>();
        inflationRecordRepository = new Mock<IInflationRecordRepository>();

        unitOfWork
            .SetupGet(x => x.InflationRecordRepository)
            .Returns(inflationRecordRepository.Object);

        useCase = new ImportCpiUseCase(ins.Object, unitOfWork.Object);
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndNullFileNameProvided_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = null
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InflationFileNotProvidedException>();
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndEmptyFileNameProvided_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = string.Empty
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InflationFileNotProvidedException>();
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndFileNameProvided_WhenImportInflations_ThenInsServiceIsCalled()
    {
        // Arrange
        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        ins.Verify(x => x.GetInflationValuesFromFile("file1"), Times.Once);
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndFileNameProvided_AndInsThrows_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .Throws<Exception>();

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InsFileException>();
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndInsReturnsItems_AndRepositoryThrows_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        inflationRecordRepository
            .Setup(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .Throws<Exception>();

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<DataStorageException>();
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndInsReturnsNoItems_WhenImportInflations_ThenRepositoryReceivesNoItems()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(new List<InflationRecordDto>());

        inflationRecordRepository
            .Setup(x => x.AddOrUpdate(It.IsAny<InflationRecord>()))
            .Throws<Exception>();

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Never);
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndInsReturnsOneItem_WhenImportInflations_ThenRepositoryReceivesOneItem()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Once);
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndInsReturnsTwoItem_WhenImportInflations_ThenRepositoryReceivesTwoItems()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto(),
                new InflationRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<InflationRecord>()), Times.Exactly(2));
    }

    [Fact]
    public async Task HavingSourceTypeFile_AndInsReturnsItems_AndRepositoryAcceptsAllItems_WhenImportInflations_ThenUnitOfWorkIsPersisted()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromFile(It.IsAny<string>()))
            .ReturnsAsync(new List<InflationRecordDto>
            {
                new InflationRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.File,
            SourceFilePath = "file1"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        unitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }

}
