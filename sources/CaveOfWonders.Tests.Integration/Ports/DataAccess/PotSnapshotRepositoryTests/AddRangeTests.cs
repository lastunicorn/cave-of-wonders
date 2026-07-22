using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class AddRangeTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task AddRange_WithSingleSnapshot_ShouldPersistSnapshot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				await backDoor.SeedPotsAsync([pot]);
				context.PotId = pot.Id;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				PotSnapshot snapshot = new()
				{
					Date = referenceDate,
					Value = 150m,
					Pot = new Pot
					{
						Id = potId
					}
				};

				repository.AddRange([snapshot]);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().HaveCount(1);
				snapshots[0].Date.Should().Be(referenceDate);
				snapshots[0].Value.Should().Be(150m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task AddRange_WithMultipleSnapshotsForSamePot_ShouldPersistAll(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				await backDoor.SeedPotsAsync([pot]);
				context.PotId = pot.Id;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				PotSnapshot[] snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 100m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 150m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				];

				repository.AddRange(snapshots);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().HaveCount(2);
				snapshots.Should().ContainSingle(x => x.Date == referenceDate.AddDays(-10) && x.Value == 100m);
				snapshots.Should().ContainSingle(x => x.Date == referenceDate && x.Value == 150m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task AddRange_WithSnapshotsForDifferentPots_ShouldPersistEachUnderTheCorrectPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				Pot pot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = referenceDate.AddDays(-30),
					Currency = "EUR"
				};

				await backDoor.SeedPotsAsync([pot1, pot2]);
				context.Pot1Id = pot1.Id;
				context.Pot2Id = pot2.Id;
			})
			.Act((repository, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				Guid pot2Id = context.Pot2Id;

				PotSnapshot[] snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 100m,
						Pot = new Pot
						{
							Id = pot1Id
						}
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 200m,
						Pot = new Pot
						{
							Id = pot2Id
						}
					}
				];

				repository.AddRange(snapshots);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				Guid pot2Id = context.Pot2Id;

				List<PotSnapshot> pot1Snapshots = await backDoor.GetSnapshotsByPotIdAsync(pot1Id);
				pot1Snapshots.Should().ContainSingle(x => x.Value == 100m);

				List<PotSnapshot> pot2Snapshots = await backDoor.GetSnapshotsByPotIdAsync(pot2Id);
				pot2Snapshots.Should().ContainSingle(x => x.Value == 200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task AddRange_WhenPotAlreadyHasSnapshots_ShouldAppendToExistingOnes(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				pot.Snapshots.Add(new PotSnapshot
				{
					Date = referenceDate.AddDays(-10),
					Value = 100m
				});

				await backDoor.SeedPotsAsync([pot]);
				context.PotId = pot.Id;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				PotSnapshot snapshot = new()
				{
					Date = referenceDate,
					Value = 150m,
					Pot = new Pot
					{
						Id = potId
					}
				};

				repository.AddRange([snapshot]);
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId = context.PotId;
				List<PotSnapshot> snapshots = await backDoor.GetSnapshotsByPotIdAsync(potId);

				snapshots.Should().HaveCount(2);
				snapshots.Should().ContainSingle(x => x.Date == referenceDate.AddDays(-10) && x.Value == 100m);
				snapshots.Should().ContainSingle(x => x.Date == referenceDate && x.Value == 150m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task AddRange_WithNullCollection_ShouldThrowArgumentNullException(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.AddRange(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}
}