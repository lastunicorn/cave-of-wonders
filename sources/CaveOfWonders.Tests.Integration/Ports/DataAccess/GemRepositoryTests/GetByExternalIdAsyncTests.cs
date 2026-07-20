using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class GetByExternalIdAsyncTests
{
	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetByExternalIdAsync_WhenDatabaseIsEmpty_ShouldReturnNull(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Gem = await repository.GetByExternalIdAsync(Guid.NewGuid(), "ext-1", CancellationToken.None);
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
	public async Task GetByExternalIdAsync_WithMatchingPotIdAndExternalId_ShouldReturnThatGem(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				Pot pot = new()
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};
				await backDoor.SeedPotAsync(pot);

				Gem gem = new()
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 5, 10),
					Category = GemCategory.Deposit,
					Amount = 150.75m,
					Description = "Initial deposit",
					Pot = new Pot
					{
						Id = potId
					}
				};
				await backDoor.SeedGemsAsync([gem]);

				context.PotId = potId;
				context.GemId = gem.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().NotBeNull();

				Guid expectedGemId = context.GemId;
				gem.Id.Should().Be(expectedGemId);
				gem.ExternalId.Should().Be("ext-1");
				gem.Date.Should().Be(new DateTime(2023, 5, 10));
				gem.Category.Should().Be(GemCategory.Deposit);
				gem.Amount.Should().Be(150.75m);
				gem.Description.Should().Be("Initial deposit");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetByExternalIdAsync_WithNonExistentExternalId_ShouldReturnNull(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "non-existent", CancellationToken.None);
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
	public async Task GetByExternalIdAsync_WithMatchingExternalIdButDifferentPotId_ShouldReturnNull(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
						ExternalId = "ext-1",
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot
						{
							Id = potId1
						}
					}
				]);

				context.PotId2 = potId2;
			})
			.Act(async (repository, context) =>
			{
				Guid potId2 = context.PotId2;
				context.Gem = await repository.GetByExternalIdAsync(potId2, "ext-1", CancellationToken.None);
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
	public async Task GetByExternalIdAsync_WithMultipleGemsHavingDifferentExternalIdsForSamePot_ShouldReturnOnlyMatchingGem(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "ext-2",
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-2", CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().NotBeNull();
				gem.ExternalId.Should().Be("ext-2");
				gem.Category.Should().Be(GemCategory.Withdrawal);
				gem.Amount.Should().Be(50m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetByExternalIdAsync_ShouldPopulateGemWithMatchingPotReference(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
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

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetByExternalIdAsync_WithGemParameters_ShouldPreserveParameters(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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

				Gem gem = new()
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				};
				gem.Parameters.Add(new GemParameter
				{
					Key = "source",
					Value = "mintos"
				});
				gem.Parameters.Add(new GemParameter
				{
					Key = "note",
					Value = "monthly transfer"
				});

				await backDoor.SeedGemsAsync([gem]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;

				gem.Should().NotBeNull();
				gem.Parameters.Should().HaveCount(2);
				gem.Parameters.Should().Contain(x => x.Key == "source" && x.Value == "mintos");
				gem.Parameters.Should().Contain(x => x.Key == "note" && x.Value == "monthly transfer");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task GetByExternalIdAsync_WithDecimalAmount_ShouldPreserveDecimalPrecision(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 123.456789m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((backDoor, context) =>
			{
				Gem gem = context.Gem;

				gem.Should().NotBeNull();
				gem.Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}
}