using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.IntegrationTests.Adapters.DataAccess.LiteDb.PotRepositoryTests;

public class GetInstancesTests
{
    private readonly DateOnly currentDate = new(2023, 7, 1);

    [Fact]
    public async Task GetInstances_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection()
    {
        await DatabaseTest.Create()
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().BeEmpty();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithActivePot_ShouldReturnPotInstance()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Pot",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-10),
                    EndDate = currentDate.AddDays(10),
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
                context.PotId = potDbEntity.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;

                potInstances.Should().HaveCount(1);

                potInstances.First().Pot.Name.Should().Be("Test Pot");

                Guid expectedPotId = context.PotId;
                potInstances[0].Pot.Id.Should().Be(expectedPotId);
                potInstances[0].Pot.Name.Should().Be("Test Pot");
                potInstances[0].Should().BeNull();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithInactivePot_ShouldNotReturnPotWhenIncludeInactiveIsFalse()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Inactive Pot - Future",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(10), // Starts in the future
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().BeEmpty();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithInactivePot_ShouldReturnPotWhenIncludeInactiveIsTrue()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Inactive Pot - Future",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(10), // Starts in the future
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
                context.PotId = potDbEntity.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, true)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(1);

                Guid expectedPotId = context.PotId;
                potInstances[0].Pot.Id.Should().Be(expectedPotId);
                potInstances[0].Pot.Name.Should().Be("Inactive Pot - Future");
                potInstances[0].Should().BeNull();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithPotEndingBeforeCurrentDate_ShouldNotReturnPotWhenIncludeInactiveIsFalse()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Inactive Pot - Past",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-20),
                    EndDate = currentDate.AddDays(-10), // Ended in the past
                    Currency = "USD"
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().BeEmpty();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithExactDateMatchingMode_ShouldReturnOnlyExactDateSnapshot()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Pot With Snapshots",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-30),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-20), Value = 100m },
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-10), Value = 150m },
                        new PotSnapshotDbEntity { Date = currentDate, Value = 200m }, // Exact match
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(10), Value = 250m }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(1);
                potInstances[0].Should().NotBeNull();
                potInstances[0].Value.Should().Be(200m);
                potInstances[0].Date.Should().Be(currentDate);
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithExactDateMatchingMode_ShouldReturnNullSnapshotWhenNoExactDateExists()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Pot Without Exact Date Snapshot",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-30),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-20), Value = 100m },
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-10), Value = 150m },
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(10), Value = 250m }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(1);
                potInstances[0].Should().BeNull();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithLastAvailableDateMatchingMode_ShouldReturnLastAvailableSnapshot()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Pot With Snapshots",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-30),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-20), Value = 100m },
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-10), Value = 150m }, // Should pick this one
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(10), Value = 250m }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.LastAvailable, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(1);
                potInstances[0].Should().NotBeNull();
                potInstances[0].Value.Should().Be(150m);
                potInstances[0].Date.Should().Be(currentDate.AddDays(-10));
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithLastAvailableDateMatchingMode_ShouldReturnNullSnapshotWhenNoSnapshotBeforeDate()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity potDbEntity = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Pot With Future Snapshots Only",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-30),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(10), Value = 100m },
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(20), Value = 150m }
                    ]
                };

                dbContext.Pots.Insert(potDbEntity);
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.LastAvailable, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(1);
                potInstances[0].Should().BeNull();
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithMultiplePots_ShouldReturnAllActivePots()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity activePot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Pot 1",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-10),
                    Currency = "USD",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate, Value = 100m }
                    ]
                };

                PotDbEntity activePot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Pot 2",
                    DisplayOrder = 2,
                    StartDate = currentDate.AddDays(-20),
                    EndDate = currentDate.AddDays(10),
                    Currency = "EUR",
                    Snapshots =
                    [
                        new PotSnapshotDbEntity { Date = currentDate.AddDays(-5), Value = 200m }
                    ]
                };

                PotDbEntity inactivePot = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Inactive Pot",
                    DisplayOrder = 3,
                    StartDate = currentDate.AddDays(10),
                    Currency = "GBP"
                };

                dbContext.Pots.Insert(activePot1);
                dbContext.Pots.Insert(activePot2);
                dbContext.Pots.Insert(inactivePot);

                context.activePot1Id = activePot1.Id;
                context.activePot2Id = activePot2.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.LastAvailable, false)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;

                potInstances.Should().HaveCount(2);

                Guid expectedActivePot1Id = context.activePot1Id;
                potInstances.Should().Contain(x => x.Pot.Id == expectedActivePot1Id);

                Guid expectedActivePot2Id = context.activePot2Id;
                potInstances.Should().Contain(x => x.Pot.Id == expectedActivePot2Id);

                PotSnapshot instance1 = potInstances.FirstOrDefault(x => x.Pot.Id == expectedActivePot1Id);
                instance1.Should().NotBeNull();
                instance1.Should().NotBeNull();
                instance1!.Value.Should().Be(100m);

                PotSnapshot instance2 = potInstances.FirstOrDefault(x => x.Pot.Id == expectedActivePot2Id);
                instance2.Should().NotBeNull();
                instance2.Should().NotBeNull();
                instance2!.Value.Should().Be(200m);
            })
            .Execute();
    }

    [Fact]
    public async Task GetInstances_WithMultiplePotsAndIncludeInactiveTrue_ShouldReturnAllPots()
    {
        await DatabaseTest.Create()
            .Arrange((dbContext, context) =>
            {
                PotDbEntity activePot = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Active Pot",
                    DisplayOrder = 1,
                    StartDate = currentDate.AddDays(-10),
                    Currency = "USD"
                };

                PotDbEntity inactivePot1 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Future Pot",
                    DisplayOrder = 2,
                    StartDate = currentDate.AddDays(10),
                    Currency = "EUR"
                };

                PotDbEntity inactivePot2 = new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Past Pot",
                    DisplayOrder = 3,
                    StartDate = currentDate.AddDays(-20),
                    EndDate = currentDate.AddDays(-5),
                    Currency = "GBP"
                };

                dbContext.Pots.Insert(activePot);
                dbContext.Pots.Insert(inactivePot1);
                dbContext.Pots.Insert(inactivePot2);

                context.ActivePotId = activePot.Id;
                context.InactivePot1Id = inactivePot1.Id;
                context.InactivePot2Id = inactivePot2.Id;
            })
            .Act(async (dbContext, context) =>
            {
                PotRepository potRepository = new(dbContext);
                List<PotSnapshot> potInstances = (await potRepository.GetSnapshots(currentDate, DateMatchingMode.Exact, true)).ToList();
                context.PotInstances = potInstances;
            })
            .Assert((dbContext, context) =>
            {
                List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
                potInstances.Should().HaveCount(3);

                Guid expectedActivePotId = context.ActivePotId;
                potInstances.Should().Contain(x => x.Pot.Id == expectedActivePotId);

                Guid expectedInactivePot1Id = context.InactivePot1Id;
                potInstances.Should().Contain(x => x.Pot.Id == expectedInactivePot1Id);

                Guid expectedInactivePot2Id = context.InactivePot2Id;
                potInstances.Should().Contain(x => x.Pot.Id == expectedInactivePot2Id);
            })
            .Execute();
    }
}