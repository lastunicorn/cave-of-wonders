using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class GetByPotIdAsyncTests
{
	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Gems = await repository.GetByPotIdAsync(Guid.NewGuid())
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithOneGemForPot_ShouldReturnThatGem(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
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
				await gateway.SeedPotAsync(pot);

				Gem gem = new()
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 5, 10),
					Category = GemCategory.Deposit,
					Amount = 150.75m,
					Description = "Initial deposit",
					Pot = new Pot { Id = potId }
				};
				await gateway.SeedGemsAsync([gem]);

				context.PotId = potId;
				context.GemId = gem.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				Gem gem = gems.First();

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
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithMultipleGemsForSamePot_ShouldReturnAllGems(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
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
				await gateway.SeedPotAsync(pot);

				Gem gem1 = new()
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				};

				Gem gem2 = new()
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 2, 10),
					Category = GemCategory.Withdrawal,
					Amount = 50m,
					Pot = new Pot { Id = potId }
				};

				Gem gem3 = new()
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 3, 10),
					Category = GemCategory.Gain,
					Amount = 25m,
					Pot = new Pot { Id = potId }
				};

				await gateway.SeedGemsAsync([gem1, gem2, gem3]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(3);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Deposit && x.Amount == 100m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Withdrawal && x.Amount == 50m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Gain && x.Amount == 25m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithGemsForDifferentPots_ShouldReturnOnlyGemsForRequestedPot(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId1,
					Name = "Pot 1",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId2,
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "EUR"
				});

				await gateway.SeedGemsAsync(
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
				context.Gems = await repository.GetByPotIdAsync(potId1)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(2);
				gems.Should().OnlyContain(x => x.Amount == 100m || x.Amount == 200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithNonExistentPotId_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await gateway.SeedGemsAsync(
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
				context.Gems = await repository.GetByPotIdAsync(Guid.NewGuid())
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_ShouldPopulateGemsWithMatchingPotReference(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Referenced Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await gateway.SeedGemsAsync(
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

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				Guid expectedPotId = context.PotId;

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Pot.Should().NotBeNull();
				gem.Pot.Id.Should().Be(expectedPotId);
				gem.Pot.Name.Should().Be("Referenced Pot");
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithGemParameters_ShouldPreserveParameters(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
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
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				};
				gem.Parameters["source"] = "mintos";
				gem.Parameters["note"] = "monthly transfer";

				await gateway.SeedGemsAsync([gem]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Parameters.Should().HaveCount(2);
				gem.Parameters.Should().Contain("source", "mintos");
				gem.Parameters.Should().Contain("note", "monthly transfer");
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithDecimalAmount_ShouldPreserveDecimalPrecision(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await gateway.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 123.456789m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithAllGemCategories_ShouldPreserveEachCategory(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				GemCategory[] categories =
				[
					GemCategory.Internal,
					GemCategory.Deposit,
					GemCategory.Withdrawal,
					GemCategory.Gain,
					GemCategory.Fee,
					GemCategory.Tax,
					GemCategory.Bonus
				];

				int day = 1;
				List<Gem> gems = categories
					.Select(category => new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, day++),
						Category = category,
						Amount = 10m,
						Pot = new Pot { Id = potId }
					})
					.ToList();

				await gateway.SeedGemsAsync(gems);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(7);
				gems.Select(x => x.Category).Should().BeEquivalentTo(
				[
					GemCategory.Internal,
					GemCategory.Deposit,
					GemCategory.Withdrawal,
					GemCategory.Gain,
					GemCategory.Fee,
					GemCategory.Tax,
					GemCategory.Bonus
				]);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task GetByPotIdAsync_WithExternalId_ShouldPreserveExternalId(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
			{
				Guid potId = Guid.NewGuid();

				await gateway.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await gateway.SeedGemsAsync(
				[
					new Gem
					{
						Id = Guid.NewGuid(),
						ExternalId = "mintos-12345",
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
				context.Gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().ExternalId.Should().Be("mintos-12345");
			})
			.ExecuteAsync();
	}
}
