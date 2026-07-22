using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class GetCountAsyncTests
{
	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetCountAsync_WhenPotHasNoGems_ShouldReturnZero(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
				context.Count = await repository.GetCountAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(0);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithOneGemForPot_ShouldReturnOne(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-1",
						Date = new DateTime(2023, 5, 10),
						Category = GemCategory.Deposit,
						Amount = 150.75m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Count = await repository.GetCountAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(1);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithMultipleGemsForSamePot_ShouldReturnTotalCount(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 3, 10),
						Category = GemCategory.Gain,
						Amount = 25m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Count = await repository.GetCountAsync(potId, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(3);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithGemsForDifferentPots_ShouldOnlyCountGemsForRequestedPot(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId1 }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot { Id = potId1 }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 3, 10),
						Category = GemCategory.Withdrawal,
						Amount = 30m,
						Pot = new Pot { Id = potId2 }
					}
				]);

				context.PotId1 = potId1;
			})
			.Act(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				context.Count = await repository.GetCountAsync(potId1, CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(2);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetCountAsync_WithNonExistentPotId_ShouldReturnZero(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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

				await backDoor.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId }
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				context.Count = await repository.GetCountAsync(Guid.NewGuid(), CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				int count = context.Count;
				count.Should().Be(0);
			})
			.ExecuteAsync();
	}
}
