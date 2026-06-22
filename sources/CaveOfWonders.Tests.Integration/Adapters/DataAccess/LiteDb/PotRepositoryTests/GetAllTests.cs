using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.LiteDb.Infrastructure;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Adapters.DataAccess.LiteDb.PotRepositoryTests;

public class GetAllTests
{
    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection()
    {
        await DatabaseTest.Create()
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = await potRepository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
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
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot",
                    Description = "This is a test pot",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = await potRepository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
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
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithMultiplePots_ShouldReturnAllPots()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity pot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 1",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD"
                };

                PotDbEntity pot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 2",
                    DisplayOrder = 2,
                    StartDate = new DateOnly(2023, 2, 1),
                    Currency = "EUR"
                };

                PotDbEntity pot3 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot 3",
                    DisplayOrder = 3,
                    StartDate = new DateOnly(2023, 3, 1),
                    Currency = "GBP"
                };

                dbContext.Pots.Insert(pot1);
                dbContext.Pots.Insert(pot2);
                dbContext.Pots.Insert(pot3);

                context.Pot1Id = pot1.Id;
                context.Pot2Id = pot2.Id;
                context.Pot3Id = pot3.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = await potRepository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
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
    public async Task GetAll_WithPotsContainingSnapshots_ShouldReturnPotsWithSnapshots()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Snapshots",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity
                        {
                            Date = new DateOnly(2023, 1, 15),
                            Value = 100.50m
                        },
                        new PotSnapshotDbEntity
                        {
                            Date = new DateOnly(2023, 2, 15),
                            Value = 120.75m
                        }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (database, context) =>
            {
                PotRepository potRepository = new(database);
                List<Pot> pots = await potRepository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((database, context) =>
            {
                List<Pot> pots = context.Pots as List<Pot>;

                pots.Should().HaveCount(1);
                Pot pot = pots.First();
                pot.Snapshots.Should().HaveCount(2);
                pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 1, 15) && x.Value == 100.50m);
                pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 2, 15) && x.Value == 120.75m);
            })
            .Execute();
    }

    [Fact]
    public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot with Labels",
                    DisplayOrder = 1,
                    StartDate = new DateOnly(2023, 1, 1),
                    Currency = "USD",
                    Labels = [
                        "Savings",
                        "Long-term",
                        "Important"
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<Pot> pots = await potRepository.GetAllAsync().ToListAsync();
                context.Pots = pots;
            })
            .Assert((dbContext, context) =>
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
