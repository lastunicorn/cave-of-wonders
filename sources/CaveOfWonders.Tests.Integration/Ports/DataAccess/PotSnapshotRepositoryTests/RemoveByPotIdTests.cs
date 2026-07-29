using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class RemoveByPotIdTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WithExistingSnapshots_ShouldRemoveAllSnapshotsOfThatPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
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
						Date = referenceDate,
						Value = 150m,
						Pot = pot
					}
				];

				await backDoor.SeedPotsAsync([pot]);
				await backDoor.SeedPotSnapshotsAsync(snapshots);
				context.PotId = pot.Id;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;
				repository.RemoveByPotId(potId);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_ShouldNotAffectSnapshotsOfOtherPots(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
				context.Pot2Id = pot2.Id;
			})
			.Act((repository, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				repository.RemoveByPotId(pot1Id);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				Guid pot2Id = context.Pot2Id;

				List<PotSnapshot> pot1Snapshots = await backDoor.GetSnapshotsByPotIdAsync(pot1Id);
				pot1Snapshots.Should().BeEmpty();

				List<PotSnapshot> pot2Snapshots = await backDoor.GetSnapshotsByPotIdAsync(pot2Id);
				pot2Snapshots.Should().ContainSingle(x => x.Value == 200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WhenPotHasNoSnapshots_ShouldNotThrow(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;
				repository.RemoveByPotId(potId);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WithUnknownPotId_ShouldNotThrow(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.RemoveByPotId(Guid.NewGuid());
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WithStartDate_ShouldRemoveOnlySnapshotsOnOrAfterThatDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				await SeedThreeSnapshots(backDoor, context);
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;
				repository.RemoveByPotId(potId, referenceDate.AddDays(-10));
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().ContainSingle(x => x.Date == referenceDate.AddDays(-20));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WithEndDate_ShouldRemoveOnlySnapshotsOnOrBeforeThatDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				await SeedThreeSnapshots(backDoor, context);
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;
				repository.RemoveByPotId(potId, null, referenceDate.AddDays(-10));
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().ContainSingle(x => x.Date == referenceDate);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task RemoveByPotId_WithStartAndEndDate_ShouldRemoveOnlySnapshotsFromThatInterval(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				await SeedThreeSnapshots(backDoor, context);
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;
				repository.RemoveByPotId(potId, referenceDate.AddDays(-15), referenceDate.AddDays(-5));
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().HaveCount(2);
				snapshots.Should().Contain(x => x.Date == referenceDate.AddDays(-20));
				snapshots.Should().Contain(x => x.Date == referenceDate);
			})
			.ExecuteAsync();
	}

	private async Task SeedThreeSnapshots(ITestBackDoor backDoor, dynamic context)
	{
		Pot pot = new()
		{
			Id = Guid.NewGuid(),
			Name = "Test Pot",
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
	}
}