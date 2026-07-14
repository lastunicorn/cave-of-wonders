using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class GetSnapshotsTests
{
	private readonly DateOnly currentDate = new(2023, 7, 1);

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.Snapshots = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;
				snapshots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithActivePot_ShouldReturnPotInstance(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-10),
					EndDate = currentDate.AddDays(10),
					Currency = "USD"
				};

				pot.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 100m
				});

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;

				potInstances.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				potInstances[0].Pot.Id.Should().Be(expectedPotId);
				potInstances[0].Pot.Name.Should().Be("Test Pot");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithInactivePot_ShouldNotReturnPotWhenIncludeInactiveIsFalse(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Inactive Pot - Future",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(10), // Starts in the future
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithInactivePot_ShouldReturnPotWhenIncludeInactiveIsTrue(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Inactive Pot - Future",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(10), // Starts in the future
					Currency = "USD"
				};

				pot.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 100m
				});

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, true);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				potInstances[0].Pot.Id.Should().Be(expectedPotId);
				potInstances[0].Pot.Name.Should().Be("Inactive Pot - Future");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithPotEndingBeforeCurrentDate_ShouldNotReturnPotWhenIncludeInactiveIsFalse(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Inactive Pot - Past",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-20),
					EndDate = currentDate.AddDays(-10), // Ended in the past
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithExactDateMatchingMode_ShouldReturnOnlyExactDateSnapshot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-30),
					Currency = "USD"
				};

				pot.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = currentDate.AddDays(-20),
						Value = 100m
					},
					new PotSnapshot
					{
						Date = currentDate.AddDays(-10),
						Value = 150m
					},
					new PotSnapshot
					{
						Date = currentDate,
						Value = 200m
					}, // Exact match
					new PotSnapshot
					{
						Date = currentDate.AddDays(10),
						Value = 250m
					}
				]);

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().HaveCount(1);
				potInstances[0].Should().NotBeNull();
				potInstances[0].Value.Should().Be(200m);
				potInstances[0].Date.Should().Be(currentDate);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithExactDateMatchingMode_ShouldNotReturnSnapshotWhenNoExactDateExists(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot Without Exact Date Snapshot",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-30),
					Currency = "USD"
				};

				pot.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = currentDate.AddDays(-20),
						Value = 100m
					},
					new PotSnapshot
					{
						Date = currentDate.AddDays(-10),
						Value = 150m
					},
					new PotSnapshot
					{
						Date = currentDate.AddDays(10),
						Value = 250m
					}
				]);

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithLastAvailableDateMatchingMode_ShouldReturnLastAvailableSnapshot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-30),
					Currency = "USD"
				};

				pot.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = currentDate.AddDays(-20),
						Value = 100m
					},
					new PotSnapshot
					{
						Date = currentDate.AddDays(-10),
						Value = 150m
					}, // Should pick this one
					new PotSnapshot
					{
						Date = currentDate.AddDays(10),
						Value = 250m
					}
				]);

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				List<PotSnapshot> potInstances = (await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.LastAvailable, false)).ToList();
				context.PotInstances = potInstances;
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().HaveCount(1);
				potInstances[0].Should().NotBeNull();
				potInstances[0].Value.Should().Be(150m);
				potInstances[0].Date.Should().Be(currentDate.AddDays(-10));
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithLastAvailableDateMatchingMode_ShouldNotReturnSnapshotWhenNoSnapshotBeforeDate(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Future Snapshots Only",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-30),
					Currency = "USD"
				};

				pot.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = currentDate.AddDays(10),
						Value = 100m
					},
					new PotSnapshot
					{
						Date = currentDate.AddDays(20),
						Value = 150m
					}
				]);

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.LastAvailable, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;
				potInstances.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithMultiplePots_ShouldReturnAllActivePots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot activePot1 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Active Pot 1",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-10),
					Currency = "USD"
				};

				activePot1.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 100m
				});

				Pot activePot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Active Pot 2",
					DisplayOrder = 2,
					StartDate = currentDate.AddDays(-20),
					EndDate = currentDate.AddDays(10),
					Currency = "EUR"
				};

				activePot2.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate.AddDays(-5),
					Value = 200m
				});

				Pot inactivePot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Inactive Pot",
					DisplayOrder = 3,
					StartDate = currentDate.AddDays(10),
					Currency = "GBP"
				};

				repository.Add(activePot1);
				repository.Add(activePot2);
				repository.Add(inactivePot);

				context.ActivePot1Id = activePot1.Id;
				context.ActivePot2Id = activePot2.Id;
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.LastAvailable, false);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
			{
				List<PotSnapshot> potInstances = context.PotInstances as List<PotSnapshot>;

				potInstances.Should().HaveCount(2);

				Guid expectedActivePot1Id = context.ActivePot1Id;
				potInstances.Should().Contain(x => x.Pot.Id == expectedActivePot1Id);

				Guid expectedActivePot2Id = context.ActivePot2Id;
				potInstances.Should().Contain(x => x.Pot.Id == expectedActivePot2Id);

				PotSnapshot instance1 = potInstances.FirstOrDefault(x => x.Pot.Id == expectedActivePot1Id);
				instance1.Should().NotBeNull();
				instance1!.Value.Should().Be(100m);

				PotSnapshot instance2 = potInstances.FirstOrDefault(x => x.Pot.Id == expectedActivePot2Id);
				instance2.Should().NotBeNull();
				instance2!.Value.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetSnapshots_WithMultiplePotsAndIncludeInactiveTrue_ShouldReturnAllPots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot activePot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Active Pot",
					DisplayOrder = 1,
					StartDate = currentDate.AddDays(-10),
					Currency = "USD"
				};

				activePot.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 100m
				});

				Pot inactivePot1 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Future Pot",
					DisplayOrder = 2,
					StartDate = currentDate.AddDays(10),
					Currency = "EUR"
				};

				inactivePot1.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 200m
				});

				Pot inactivePot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Past Pot",
					DisplayOrder = 3,
					StartDate = currentDate.AddDays(-20),
					EndDate = currentDate.AddDays(-5),
					Currency = "GBP"
				};

				inactivePot2.Snapshots.Add(new PotSnapshot
				{
					Date = currentDate,
					Value = 300m
				});

				repository.Add(activePot);
				repository.Add(inactivePot1);
				repository.Add(inactivePot2);

				context.ActivePotId = activePot.Id;
				context.InactivePot1Id = inactivePot1.Id;
				context.InactivePot2Id = inactivePot2.Id;
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<PotSnapshot> snapshotEnumerable = await repository.GetSnapshotsAsync(currentDate, DateMatchingMode.Exact, true);
				context.PotInstances = snapshotEnumerable.ToList();
			})
			.Assert((repository, context) =>
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
			.ExecuteAsync();
	}
}