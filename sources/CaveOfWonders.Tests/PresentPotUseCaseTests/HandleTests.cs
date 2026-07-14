using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.PresentPotUseCaseTests;

public class HandleTests
{
    private readonly PresentPotUseCase useCase;
    private readonly Mock<IPotRepository> potRepository;

    public HandleTests()
    {
        Mock<IUnitOfWork> unitOfWork = new();
        Mock<ISystemClock> systemClock = new();

        potRepository = new Mock<IPotRepository>();

        unitOfWork
            .Setup(x => x.PotRepository)
            .Returns(potRepository.Object);

        useCase = new PresentPotUseCase(unitOfWork.Object, systemClock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public async Task HavingNullOrEmptyOrWhiteSpaceIdentifier_WhenPresentingPot_ThenDoesNotThrow(string potIdentifier)
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = potIdentifier
        };

        potRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Array.Empty<Pot>().ToAsyncEnumerable());

        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Returns(Array.Empty<Pot>().ToAsyncEnumerable());

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HavingPotIdentifierSpecified_WhenPresentingPot_ThenRepositoryIsCalledWithThatPotIdentifier()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Returns(Array.Empty<Pot>().ToAsyncEnumerable());

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        potRepository
            .Verify(x => x.GetAsync(request.PotFlexId, CancellationToken.None), Times.Once, "Repository should be called with the specified pot identifier.");
    }

    [Fact]
    public async Task HavingRepositoryInaccessible_WhenPresentingPot_ThenThrows()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = "dummy-id"
        };
        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("Repository is inaccessible."));

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<StorageInaccessibleException>();
    }

    [Fact]
    public async Task HavingNoPotsInRepository_WhenPresentingPot_ThenReturnsEmptyList()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Returns(Array.Empty<Pot>().ToAsyncEnumerable());

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.PotDetails.Should().BeEmpty("No pots should be returned when the repository is empty.");
    }

    [Fact]
    public async Task HavingOneMatchingPotInRepository_WhenPresentingPot_ThenReturnsOnePot()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Returns(new List<Pot> { new() }.ToAsyncEnumerable());

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.PotDetails.Should().HaveCount(1, "One pot should be returned when there is one matching pot in the repository.");
    }

    [Fact]
    public async Task HavingTwoMatchingPotsInRepository_WhenPresentingPot_ThenReturnsTwoPots()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotFlexId = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetAsync(It.IsAny<PotFlexId>(), It.IsAny<CancellationToken>()))
            .Returns(new List<Pot> { new(), new() }.ToAsyncEnumerable());

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.PotDetails.Should().HaveCount(2, "Two pots should be returned when there are two matching pots in the repository.");
    }
}
