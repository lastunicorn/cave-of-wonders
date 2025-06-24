using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
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
    public async Task HavingNullOrEmptyOrWhiteSpaceIdentifier_WhenPresentingPot_ThenThrows(string potIdentifier)
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = potIdentifier
        };

        // Act
        Func<Task> action = async () => await useCase.Handle(request, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<PotIdentifierNotSpecifiedException>();
    }

    [Fact]
    public async Task HavingPotIdentifierSpecified_WhenPresentingPot_ThenRepositoryIsCalledWithThatPotIdentifier()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = "dummy-id"
        };

        // Act
        _ = await useCase.Handle(request, CancellationToken.None);

        // Assert
        potRepository
            .Verify(x => x.GetByIdOrName(request.PotIdentifier), Times.Once, "Repository should be called with the specified pot identifier.");
    }

    [Fact]
    public async Task HavingRepositoryInaccessible_WhenPresentingPot_ThenThrows()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = "dummy-id"
        };
        potRepository
            .Setup(x => x.GetByIdOrName(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Repository is inaccessible."));

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
            PotIdentifier = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetByIdOrName(It.IsAny<string>()))
            .ReturnsAsync([]);

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.Pots.Should().BeEmpty("No pots should be returned when the repository is empty.");
    }

    [Fact]
    public async Task HavingOneMatchingPotInRepository_WhenPresentingPot_ThenReturnsOnePot()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetByIdOrName(It.IsAny<string>()))
            .ReturnsAsync([new Pot()]);

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.Pots.Should().HaveCount(1, "One pot should be returned when there is one matching pot in the repository.");
    }

    [Fact]
    public async Task HavingTwoMatchingPotsInRepository_WhenPresentingPot_ThenReturnsTwoPots()
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetByIdOrName(It.IsAny<string>()))
            .ReturnsAsync([
                new Pot(),
                new Pot()
            ]);

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert
        response.Pots.Should().HaveCount(2, "Two pots should be returned when there are two matching pots in the repository.");
    }
}
