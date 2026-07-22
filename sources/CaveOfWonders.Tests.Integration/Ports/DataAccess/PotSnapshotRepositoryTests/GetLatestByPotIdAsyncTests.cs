using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotSnapshotRepositoryTests;

public class GetLatestByPotIdAsyncTests
{
	private readonly DateOnly referenceDate = new(2023, 7, 1);

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetLatestByPotIdAsync_WhenPotHasNoSnapshots_ShouldReturnNull(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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
				context.LatestSnapshot = await repository.GetLatestByPotIdAsync(potId);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot latestSnapshot = context.LatestSnapshot;
				latestSnapshot.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetLatestByPotIdAsync_WithUnknownPotId_ShouldReturnNull(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.LatestSnapshot = await repository.GetLatestByPotIdAsync(Guid.NewGuid());
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot latestSnapshot = context.LatestSnapshot;
				latestSnapshot.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetLatestByPotIdAsync_WithMultipleSnapshots_ShouldReturnTheOneWithMaxDate(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				pot.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-20),
						Value = 100m
					},
					new PotSnapshot
					{
						Date = referenceDate,
						Value = 300m
					}, // most recent, inserted out of order
					new PotSnapshot
					{
						Date = referenceDate.AddDays(-10),
						Value = 200m
					}
				]);

				await backDoor.SeedPotsAsync([pot]);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.LatestSnapshot = await repository.GetLatestByPotIdAsync(potId);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot latestSnapshot = context.LatestSnapshot;

				latestSnapshot.Should().NotBeNull();
				latestSnapshot.Date.Should().Be(referenceDate);
				latestSnapshot.Value.Should().Be(300m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotSnapshotRepository, ITestBackDoor>]
	public async Task GetLatestByPotIdAsync_WithMultiplePots_ShouldOnlyConsiderSnapshotsOfRequestedPot(ITestEnvironment<IPotSnapshotRepository, ITestBackDoor> environment)
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

				pot1.Snapshots.Add(new PotSnapshot
				{
					Date = referenceDate.AddDays(-10),
					Value = 100m
				});

				Pot pot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = referenceDate.AddDays(-30),
					Currency = "EUR"
				};

				pot2.Snapshots.Add(new PotSnapshot
				{
					Date = referenceDate,
					Value = 200m
				});

				await backDoor.SeedPotsAsync([pot1, pot2]);
				context.Pot1Id = pot1.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid pot1Id = context.Pot1Id;
				context.LatestSnapshot = await repository.GetLatestByPotIdAsync(pot1Id);
			})
			.Assert((backDoor, context) =>
			{
				PotSnapshot latestSnapshot = context.LatestSnapshot;

				latestSnapshot.Should().NotBeNull();
				latestSnapshot.Date.Should().Be(referenceDate.AddDays(-10));
				latestSnapshot.Value.Should().Be(100m);
			})
			.ExecuteAsync();
	}
}