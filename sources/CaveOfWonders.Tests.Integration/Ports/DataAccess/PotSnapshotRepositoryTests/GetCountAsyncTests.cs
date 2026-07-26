using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class GetCountAsyncTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetCountAsync_WhenPotHasNoSnapshots_ShouldReturnZero(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
				context.Count = await repository.GetCountAsync(potId);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(0);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithUnknownPotId_ShouldReturnZero(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Count = await repository.GetCountAsync(Guid.NewGuid());
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(0);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithMultipleSnapshots_ShouldReturnTheirCount(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
				context.Count = await repository.GetCountAsync(potId);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(3);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithMultiplePots_ShouldOnlyCountSnapshotsOfRequestedPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				List<PotSnapshot> pot2Snapshots =
				[
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m,
						Pot = pot2
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 250m,
						Pot = pot2
					}
				];

				await backDoor.SeedPotsAsync([pot1, pot2]);
				await backDoor.SeedPotSnapshotsAsync([pot1Snapshot, ..pot2Snapshots]);
				context.Pot1Id = pot1.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				context.Count = await repository.GetCountAsync(pot1Id);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(1);
			})
			.ExecuteAsync();
	}
}