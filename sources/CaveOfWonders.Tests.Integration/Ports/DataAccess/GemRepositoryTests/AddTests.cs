using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class AddTests
{
	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithValidGem_ShouldPersistGem(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

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

				repository.Add(gem);

				context.GemId = gem.Id;
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

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
	[GemRepositoryProviders]
	public async Task Add_WithNullGem_ShouldThrowArgumentNullException(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Act((repository, context) =>
			{
				repository.Add(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithMultipleGems_ShouldPersistAllOfThem(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 2, 10),
					Category = GemCategory.Withdrawal,
					Amount = 50m,
					Pot = new Pot
					{
						Id = potId
					}
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 3, 10),
					Category = GemCategory.Gain,
					Amount = 25m,
					Pot = new Pot
					{
						Id = potId
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

				gems.Should().HaveCount(3);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Deposit && x.Amount == 100m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Withdrawal && x.Amount == 50m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Gain && x.Amount == 25m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithGemsForDifferentPots_ShouldPersistEachUnderItsOwnPot(IGemRepositorySutFixture sutFixture)
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

				context.PotId1 = potId1;
				context.PotId2 = potId2;
			})
			.Act((repository, context) =>
			{
				Guid potId1 = context.PotId1;
				Guid potId2 = context.PotId2;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId1
					}
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 11),
					Category = GemCategory.Withdrawal,
					Amount = 30m,
					Pot = new Pot
					{
						Id = potId2
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				Guid potId2 = context.PotId2;

				List<Gem> gemsForPot1 = await repository.GetByPotIdAsync(potId1)
					.ToListAsync();
				List<Gem> gemsForPot2 = await repository.GetByPotIdAsync(potId2)
					.ToListAsync();

				gemsForPot1.Should().HaveCount(1);
				gemsForPot1.First().Amount.Should().Be(100m);

				gemsForPot2.Should().HaveCount(1);
				gemsForPot2.First().Amount.Should().Be(30m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_ShouldPersistPotReference(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Pot.Should().NotBeNull();
				gem.Pot.Id.Should().Be(potId);
				gem.Pot.Name.Should().Be("Referenced Pot");
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithGemParameters_ShouldPersistParameters(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				Gem gem = new()
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				};
				gem.Parameters["source"] = "mintos";
				gem.Parameters["note"] = "monthly transfer";

				repository.Add(gem);
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Parameters.Should().HaveCount(2);
				gem.Parameters.Should().Contain("source", "mintos");
				gem.Parameters.Should().Contain("note", "monthly transfer");
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithDecimalAmount_ShouldPreserveDecimalPrecision(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 123.456789m,
					Pot = new Pot
					{
						Id = potId
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithAllGemCategories_ShouldPersistEachCategory(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

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
				foreach (GemCategory category in categories)
				{
					repository.Add(new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, day++),
						Category = category,
						Amount = 10m,
						Pot = new Pot
						{
							Id = potId
						}
					});
				}
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

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
	[GemRepositoryProviders]
	public async Task Add_WithoutExternalId_ShouldPersistGemWithNullExternalId(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();

				gems.Should().HaveCount(1);
				gems.First().ExternalId.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task Add_WithGem_ShouldBeRetrievableByExternalId(IGemRepositorySutFixture sutFixture)
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

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					ExternalId = "mintos-12345",
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot
					{
						Id = potId
					}
				});
			})
			.Assert(async (repository, context) =>
			{
				Guid potId = context.PotId;
				Gem gem = await repository.GetByExternalIdAsync(potId, "mintos-12345", CancellationToken.None);

				gem.Should().NotBeNull();
				gem.ExternalId.Should().Be("mintos-12345");
				gem.Amount.Should().Be(100m);
			})
			.ExecuteAsync();
	}
}