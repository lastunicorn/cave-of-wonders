using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportCpiUseCaseTests;

public class ImportSourceWebTests
{
    private readonly ImportCpiUseCase useCase;
    private readonly Mock<IInsService> ins;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<ICpiRepository> inflationRecordRepository;

    public ImportSourceWebTests()
    {
        ins = new Mock<IInsService>();
        unitOfWork = new Mock<IUnitOfWork>();
        inflationRecordRepository = new Mock<ICpiRepository>();

        unitOfWork
            .SetupGet(x => x.CpiRepository)
            .Returns(inflationRecordRepository.Object);

        useCase = new ImportCpiUseCase(unitOfWork.Object, null);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_WhenImportInflations_ThenInsServiceIsCalled()
    {
        // Arrange
        ImportCpiRequest request = new()
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

        ImportCpiRequest request = new()
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
            .ReturnsAsync(new List<CpiRecordDto>
            {
                new CpiRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<Cpi>()), Times.Once);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsTwoItems_WhenImportInflations_ThenRepositoryReceivesTwoItems()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<CpiRecordDto>
            {
                new CpiRecordDto(),
                new CpiRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<Cpi>()), Times.Exactly(2));
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsNoItem_WhenImportInflations_ThenRepositoryIsNotCalled()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<CpiRecordDto>());

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        inflationRecordRepository.Verify(x => x.AddOrUpdate(It.IsAny<Cpi>()), Times.Never);
    }

    [Fact]
    public async Task HavingSourceTypeWeb_AndInsReturnsItems_AndRepositoryThrows_WhenImportInflations_ThenThrows()
    {
        // Arrange
        ins
            .Setup(x => x.GetInflationValuesFromWeb())
            .ReturnsAsync(new List<CpiRecordDto>
            {
                new CpiRecordDto()
            });

        inflationRecordRepository
            .Setup(x => x.AddOrUpdate(It.IsAny<Cpi>()))
            .Throws<Exception>();

        ImportCpiRequest request = new()
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
            .ReturnsAsync(new List<CpiRecordDto>
            {
                new CpiRecordDto()
            });

        ImportCpiRequest request = new()
        {
            ImportSource = ImportSource.Web
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        unitOfWork.Verify(x => x.SaveChanges(), Times.Once);
    }
}
