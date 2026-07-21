using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class GetLatestAsyncTests
{
	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetLatestAsync_WhenPotHasNoGems_ShouldReturnNull(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetLatestAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetLatestAsync_WithMultipleGemsForPot_ShouldReturnGemWithMostRecentDate(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				Guid latestGemId = Guid.NewGuid();

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-1",
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = latestGemId,
						ExternalId = "ext-3",
						Date = new DateTime(2023, 5, 20),
						Category = GemCategory.Gain,
						Amount = 25m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-2",
						Date = new DateTime(2023, 3, 15),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
				context.LatestGemId = latestGemId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetLatestAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				Guid expectedGemId = context.LatestGemId;

				gem.Should().NotBeNull();
				gem.Id.Should().Be(expectedGemId);
				gem.ExternalId.Should().Be("ext-3");
				gem.Date.Should().Be(new DateTime(2023, 5, 20));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetLatestAsync_WithGemsFromMultiplePots_ShouldOnlyConsiderGemsForRequestedPot(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId1,
					Name = "Pot 1",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId2,
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "EUR"
				});

				Guid expectedGemId = Guid.NewGuid();

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = expectedGemId,
						ExternalId = "ext-1",
						Date = new DateTime(2023, 2, 1),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId1 }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-2",
						Date = new DateTime(2023, 6, 1),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot { Id = potId2 }
					}
				]);

				context.PotId1 = potId1;
				context.ExpectedGemId = expectedGemId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				context.Gem = await repository.GetLatestAsync(potId1, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				Guid expectedGemId = context.ExpectedGemId;

				gem.Should().NotBeNull();
				gem.Id.Should().Be(expectedGemId);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetLatestAsync_ShouldPopulateGemWithMatchingPotReference(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Referenced Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-1",
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetLatestAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				Guid expectedPotId = context.PotId;

				gem.Should().NotBeNull();
				gem.Pot.Should().NotBeNull();
				gem.Pot.Id.Should().Be(expectedPotId);
				gem.Pot.Name.Should().Be("Referenced Pot");
			})
			.ExecuteAsync();
	}
}
