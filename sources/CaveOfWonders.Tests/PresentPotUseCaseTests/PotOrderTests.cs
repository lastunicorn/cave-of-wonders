using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.PresentPotUseCaseTests;

public class PotOrderTests
{
    private readonly PresentPotUseCase useCase;
    private readonly Mock<IPotRepository> potRepository;

    public PotOrderTests()
    {
        Mock<IUnitOfWork> unitOfWork = new();
        Mock<ISystemClock> systemClock = new();

        potRepository = new Mock<IPotRepository>();

        unitOfWork
            .Setup(x => x.PotRepository)
            .Returns(potRepository.Object);

        useCase = new PresentPotUseCase(unitOfWork.Object, systemClock.Object);
    }

    [Fact]
    public async Task HavingTwoMatchingPotsInOrderInRepository_WhenPresentingPot_ThenReturnsTwoPotsInOrder()
    {
        List<Pot> potsFromRepository = [
            new Pot
            {
                Id = new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
                DisplayOrder = 1
            },
            new Pot
            {
                Id = new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
                DisplayOrder = 2
            }
        ];

        Guid[] expectedIds = [
            new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
            new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4")
        ];

        await PerformTest(potsFromRepository, expectedIds);
    }


    [Fact]
    public async Task HavingTwoMatchingPotsOutOfOrderInRepository_WhenPresentingPot_ThenReturnsTwoPotsInOrder()
    {
        List<Pot> potsFromRepository = [
            new Pot
            {
                Id = new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
                DisplayOrder = 2
            },
            new Pot
            {
                Id = new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
                DisplayOrder = 1
            }
        ];

        Guid[] expectedIds = [
            new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
            new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7")
        ];

        await PerformTest(potsFromRepository, expectedIds);
    }

    [Fact]
    public async Task HavingThreeMatchingPotsInReverseOrderInRepository_WhenPresentingPot_ThenReturnsThreePotsInOrder()
    {
        List<Pot> potsFromRepository = [
            new Pot
            {
                Id = new Guid("09FAC314-AFA9-4EED-BE89-B5D0E5D41656"),
                DisplayOrder = 3
            },
            new Pot
            {
                Id = new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
                DisplayOrder = 2
            },
            new Pot
            {
                Id = new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
                DisplayOrder = 1
            }
        ];

        Guid[] expectedIds = [
            new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
            new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
            new Guid("09FAC314-AFA9-4EED-BE89-B5D0E5D41656")
        ];

        await PerformTest(potsFromRepository, expectedIds);
    }

    [Fact]
    public async Task HavingThreeMatchingPotsInScrambledOrderInRepository_WhenPresentingPot_ThenReturnsThreePotsInOrder()
    {
        List<Pot> potsFromRepository = [
            new Pot
            {
                Id = new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
                DisplayOrder = 2
            },
            new Pot
            {
                Id = new Guid("09FAC314-AFA9-4EED-BE89-B5D0E5D41656"),
                DisplayOrder = 3
            },
            new Pot
            {
                Id = new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
                DisplayOrder = 1
            }
        ];

        Guid[] expectedIds = [
            new Guid("B56B21E3-E99C-43D7-B9B9-8E0E1CBF43B7"),
            new Guid("6B810E80-3814-4454-B16E-5CBE17594FC4"),
            new Guid("09FAC314-AFA9-4EED-BE89-B5D0E5D41656")
        ];

        await PerformTest(potsFromRepository, expectedIds);
    }

    private async Task PerformTest(List<Pot> potsFromRepository, Guid[] expectedIds)
    {
        // Arrange
        PresentPotRequest request = new()
        {
            PotIdentifier = "dummy-id"
        };

        potRepository
            .Setup(x => x.GetByIdOrName(It.IsAny<string>()))
            .ReturnsAsync(potsFromRepository);

        // Act
        PresentPotResponse response = await useCase.Handle(request, CancellationToken.None);

        // Assert

        Guid[] actualIds = response.Pots
            .Select(x => x.Id)
            .ToArray();

        actualIds.Should().BeEquivalentTo(expectedIds, options => options.WithStrictOrdering());
    }
}