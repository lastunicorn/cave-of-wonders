using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class GetNextAsyncTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WhenPotHasNoSnapshots_ShouldReturnNull(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot Without Snapshots",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				await backDoor.SeedPotsAsync([pot]);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.NextSnapshot = await repository.GetNextAsync(potId, referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;
				nextSnapshot.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WithUnknownPotId_ShouldReturnNull(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.NextSnapshot = await repository.GetNextAsync(Guid.NewGuid(), referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;
				nextSnapshot.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WhenAllSnapshotsAreBeforeDate_ShouldReturnNull(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				PotSnapshot snapshot = new()
				{
					Date = referenceDate.AddDays(-10),
					Value = 100m,
					Pot = pot
				};

				await backDoor.SeedPotsAsync([pot]);
				await backDoor.SeedPotSnapshotsAsync([snapshot]);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.NextSnapshot = await repository.GetNextAsync(potId, referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;
				nextSnapshot.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WithSnapshotExactlyOnDate_ShouldReturnThatSnapshot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 200m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(10),
						Value = 100m,
						Pot = pot
					}
				];

				await backDoor.SeedPotsAsync([pot]);
				await backDoor.SeedPotSnapshotsAsync(snapshots);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.NextSnapshot = await repository.GetNextAsync(potId, referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;

				nextSnapshot.Should().NotBeNull();
				nextSnapshot.Date.Should().Be(referenceDate);
				nextSnapshot.Value.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WithMultipleSnapshotsAfterDate_ShouldReturnTheClosestOne(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 100m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(5),
						Value = 300m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(20),
						Value = 400m,
						Pot = pot
					}
				];

				await backDoor.SeedPotsAsync([pot]);
				await backDoor.SeedPotSnapshotsAsync(snapshots);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.NextSnapshot = await repository.GetNextAsync(potId, referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;

				nextSnapshot.Should().NotBeNull();
				nextSnapshot.Date.Should().Be(referenceDate.AddDays(5));
				nextSnapshot.Value.Should().Be(300m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetNextAsync_WithMultiplePots_ShouldOnlyConsiderSnapshotsOfRequestedPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot1 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot 1",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				PotSnapshot pot1Snapshot = new()
				{
					Date = referenceDate.AddDays(10),
					Value = 100m,
					Pot = pot1
				};

				Pot pot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = referenceDate.AddDays(-30),
					Currency = "EUR"
				};

				PotSnapshot pot2Snapshot = new()
				{
					Date = referenceDate.AddDays(5),
					Value = 200m,
					Pot = pot2
				};

				await backDoor.SeedPotsAsync([pot1, pot2]);
				await backDoor.SeedPotSnapshotsAsync([pot1Snapshot, pot2Snapshot]);
				context.Pot1Id = pot1.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				context.NextSnapshot = await repository.GetNextAsync(pot1Id, referenceDate);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot nextSnapshot = context.NextSnapshot;

				nextSnapshot.Should().NotBeNull();
				nextSnapshot.Date.Should().Be(referenceDate.AddDays(10));
				nextSnapshot.Value.Should().Be(100m);
			})
			.ExecuteAsync();
	}
}
