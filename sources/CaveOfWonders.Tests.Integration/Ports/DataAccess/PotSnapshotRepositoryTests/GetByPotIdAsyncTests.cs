using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class GetByPotIdAsyncTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WhenPotHasNoSnapshots_ShouldReturnEmptyCollection(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(potId).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;
				snapshots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithUnknownPotId_ShouldReturnEmptyCollection(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(Guid.NewGuid()).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;
				snapshots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithMultipleSnapshots_ShouldReturnAllOrderedByDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 300m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-20),
						Value = 100m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m,
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
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(potId).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;

				snapshots.Should().HaveCount(3);
				snapshots.Select(x => x.Value).Should().ContainInOrder(100m, 200m, 300m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithStartDate_ShouldExcludeSnapshotsBeforeStartDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-20),
						Value = 100m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 300m,
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
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(potId, startDate: referenceDate.AddDays(-10)).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;

				snapshots.Should().HaveCount(2);
				snapshots.Select(x => x.Value).Should().ContainInOrder(200m, 300m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithEndDate_ShouldExcludeSnapshotsAfterEndDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-20),
						Value = 100m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 300m,
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
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(potId, endDate: referenceDate.AddDays(-10)).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;

				snapshots.Should().HaveCount(2);
				snapshots.Select(x => x.Value).Should().ContainInOrder(100m, 200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithStartDateAndEndDate_ShouldReturnOnlySnapshotsWithinRange(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot With Snapshots",
					DisplayOrder = 1,
					StartDate = referenceDate.AddDays(-30),
					Currency = "USD"
				};

				List<PotSnapshot> snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-20),
						Value = 100m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 300m,
						Pot = pot
					},
					new PotSnapshot
					{
						Date = referenceDate.AddDays(10),
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

				List<PotSnapshot> snapshots = await repository
					.GetByPotIdAsync(potId, startDate: referenceDate.AddDays(-10), endDate: referenceDate)
					.ToListAsync();

				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;

				snapshots.Should().HaveCount(2);
				snapshots.Select(x => x.Value).Should().ContainInOrder(200m, 300m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetByPotIdAsync_WithMultiplePots_ShouldOnlyReturnSnapshotsOfRequestedPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
					Date = referenceDate,
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
					Date = referenceDate,
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
				List<PotSnapshot> snapshots = await repository.GetByPotIdAsync(pot1Id).ToListAsync();
				context.Snapshots = snapshots;
			})
			.Assert((backDoor, context) =>
			{
				List<PotSnapshot> snapshots = context.Snapshots as List<PotSnapshot>;

				snapshots.Should().HaveCount(1);
				snapshots[0].Value.Should().Be(100m);
			})
			.ExecuteAsync();
	}
}
