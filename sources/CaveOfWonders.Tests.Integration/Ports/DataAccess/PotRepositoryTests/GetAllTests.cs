using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Providers;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class GetAllTests
{
    [Theory]
    [PotRepositoryProviders]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ISutProvider<IPotRepository> provider)
    {
        await new GenericTest<IPotRepository>(provider)
            .Act(async (repository, context) =>
            {
                List<Pot> pots = await repository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((repository, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;
                pots.Should().BeEmpty();
            })
            .ExecuteAsync();
    }

    [Theory]
    [PotRepositoryProviders]
    public async Task GetAll_WithOnePot_ShouldReturnOnePot(ISutProvider<IPotRepository> provider)
    {
        await new GenericTest<IPotRepository>(provider)
            .Arrange((repository, context) =>
            {
                Pot potInDb = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot",
                    Description = "This is a test pot",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD"
                };
                repository.Add(potInDb);
            })
            .Act(async (repository, context) =>
            {
                List<Pot> pots = await repository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((repository, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Id.Should().NotBeEmpty();
                pot.Name.Should().Be("Test Pot");
                pot.Description.Should().Be("This is a test pot");
                pot.DisplayOrder.Should().Be(1);
                pot.StartDate.Should().Be(new DateOnly(2023, 1, 1));
                pot.Currency.Should().Be("USD");
            })
            .ExecuteAsync();
    }

    [Theory]
    [PotRepositoryProviders]
    public async Task GetAll_WithMultiplePots_ShouldReturnAllPots(ISutProvider<IPotRepository> provider)
    {
        await new GenericTest<IPotRepository>(provider)
            .Arrange((repository, context) =>
            {
                Pot pot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 1",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD"
                };

                Pot pot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 2",
                    DisplayOrder = 2,
                    StartDate = new DateOnly(2023, 2, 1),
                    Currency = "EUR"
                };

                Pot pot3 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 3",
                    DisplayOrder = 3,
                    StartDate = new DateOnly(2023, 3, 1),
                    Currency = "GBP"
                };

                repository.Add(pot1);
                repository.Add(pot2);
                repository.Add(pot3);

                context.Pot1Id = pot1.Id;
                context.Pot2Id = pot2.Id;
                context.Pot3Id = pot3.Id;
            })
            .Act(async (repository, context) =>
            {
                List<Pot> pots = await repository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((repository, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(3);

                Guid expectedPot1Id = (Guid)context.Pot1Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot1Id && x.Name == "Test Pot 1");

                Guid expectedPot2Id = (Guid)context.Pot2Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot2Id && x.Name == "Test Pot 2");

                Guid expectedPot3Id = (Guid)context.Pot3Id;
                pots.Should().ContainSingle(x => x.Id == expectedPot3Id && x.Name == "Test Pot 3");
            })
            .ExecuteAsync();
    }
}
