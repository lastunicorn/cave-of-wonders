using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class GetByExternalIdAsyncTests
{
	[Theory]
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WhenDatabaseIsEmpty_ShouldReturnNull(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Act(async (repository, context) =>
			{
				context.Gem = await repository.GetByExternalIdAsync(Guid.NewGuid(), "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithMatchingPotIdAndExternalId_ShouldReturnThatGem(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
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
				sutFixture.SeedPot(pot);

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
				repository.Add(gem);

				context.PotId = potId;
				context.GemId = gem.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithNonExistentExternalId_ShouldReturnNull(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "non-existent", CancellationToken.None);
			})
			.Assert((repository, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithMatchingExternalIdButDifferentPotId_ShouldReturnNull(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
				{
					Id = potId1,
					Name = "Pot 1",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				sutFixture.SeedPot(new Pot
				{
					Id = potId2,
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "EUR"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId1 }
				});

				context.PotId2 = potId2;
			})
			.Act(async (repository, context) =>
			{
				Guid potId2 = context.PotId2;
				context.Gem = await repository.GetByExternalIdAsync(potId2, "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
			{
				Gem gem = context.Gem;
				gem.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithMultipleGemsHavingDifferentExternalIdsForSamePot_ShouldReturnOnlyMatchingGem(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-2",
					Date = new DateTime(2023, 2, 10),
					Category = GemCategory.Withdrawal,
					Amount = 50m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-2", CancellationToken.None);
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_ShouldPopulateGemWithMatchingPotReference(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
				{
					Id = potId,
					Name = "Referenced Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithGemParameters_ShouldPreserveParameters(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
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
					Pot = new Pot { Id = potId }
				};
				gem.Parameters["source"] = "mintos";
				gem.Parameters["note"] = "monthly transfer";

				repository.Add(gem);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
			{
				Gem gem = context.Gem;

				gem.Should().NotBeNull();
				gem.Parameters.Should().HaveCount(2);
				gem.Parameters.Should().Contain("source", "mintos");
				gem.Parameters.Should().Contain("note", "monthly transfer");
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task GetByExternalIdAsync_WithDecimalAmount_ShouldPreserveDecimalPrecision(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				sutFixture.SeedPot(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "ext-1",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 123.456789m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gem = await repository.GetByExternalIdAsync(potId, "ext-1", CancellationToken.None);
			})
			.Assert((repository, context) =>
			{
				Gem gem = context.Gem;

				gem.Should().NotBeNull();
				gem.Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}
}
