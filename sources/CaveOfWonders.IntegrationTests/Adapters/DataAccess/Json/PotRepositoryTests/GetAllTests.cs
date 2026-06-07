using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.Json.Infrastructure;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.Json.PotRepositoryTests;

public class GetAllTests
{
    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection()
    {
        await DatabaseTest.Create()
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;
                pots.Should().BeEmpty();
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithOnePot_ShouldReturnOnePot()
    {
        await DatabaseTest.Create()
            .Arrange((database, context) =>
            {
                Pot potInDb = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot",
                    Description = "This is a test pot",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };
                database.Pots.Add(potInDb);
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Id.Should().NotBeEmpty();
                pot.Name.Should().Be("Test Pot");
                pot.Description.Should().Be("This is a test pot");
                pot.DisplayOrder.Should().Be(1);
                pot.StartDate.Should().Be(new DateTime(2023, 1, 1));
                pot.Currency.Should().Be("USD");
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithMultiplePots_ShouldReturnAllPots()
    {
        await DatabaseTest.Create()
            .Arrange((database, context) =>
            {
                Pot pot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 1",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };

                Pot pot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 2",
                    DisplayOrder = 2,
                    StartDate = new DateTime(2023, 2, 1),
                    Currency = "EUR"
                };

                Pot pot3 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 3",
                    DisplayOrder = 3,
                    StartDate = new DateTime(2023, 3, 1),
                    Currency = "GBP"
                };

                database.Pots.Add(pot1);
                database.Pots.Add(pot2);
                database.Pots.Add(pot3);

                context.Pot1Id = pot1.Id;
                context.Pot2Id = pot2.Id;
                context.Pot3Id = pot3.Id;
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
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
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithPotsContainingGems_ShouldReturnPotsWithGems()
    {
        await DatabaseTest.Create()
            .Arrange((database, context) =>
            {
                Pot potInDb = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Gems",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };

                potInDb.Gems.AddRange([
                    new Gem
                    {
                        Date = new DateTime(2023, 1, 15),
                        Value = 100.50m
                    },
                    new Gem
                    {
                        Date = new DateTime(2023, 2, 15),
                        Value = 120.75m
                    }
                ]);

                database.Pots.Add(potInDb);
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Gems.Should().HaveCount(2);
                pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 1, 15) && x.Value == 100.50m);
                pot.Gems.Should().ContainSingle(x => x.Date == new DateTime(2023, 2, 15) && x.Value == 120.75m);
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels()
    {
        await DatabaseTest.Create()
            .Arrange((database, context) =>
            {
                Pot potInDb = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Labels",
                    DisplayOrder = 1,
                    StartDate = new DateTime(2023, 1, 1),
                    Currency = "USD"
                };

                potInDb.Labels.AddRange([
                    "Savings",
                    "Long-term",
                    "Important"
                ]);

                database.Pots.Add(potInDb);
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = (await potRepository.GetAll()).ToList();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Labels.Should().HaveCount(3);
                pot.Labels.Should().Contain("Savings");
                pot.Labels.Should().Contain("Long-term");
                pot.Labels.Should().Contain("Important");
            })
            .Execute();
    }
}
