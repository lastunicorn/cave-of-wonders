using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class AddTests
{
	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithValidGem_ShouldPersistGem(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithNullGem_ShouldThrowArgumentNullException(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithMultipleGems_ShouldPersistAllOfThem(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().HaveCount(3);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Deposit && x.Amount == 100m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Withdrawal && x.Amount == 50m);
				gems.Should().ContainSingle(x => x.Category == GemCategory.Gain && x.Amount == 25m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithGemsForDifferentPots_ShouldPersistEachUnderItsOwnPot(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				Guid potId1 = context.PotId1;
				Guid potId2 = context.PotId2;

				List<Gem> gems = await gateway.GetAllGemsAsync();

				List<Gem> gemsForPot1 = gems.Where(x => x.Pot.Id == potId1).ToList();
				List<Gem> gemsForPot2 = gems.Where(x => x.Pot.Id == potId2).ToList();

				gemsForPot1.Should().HaveCount(1);
				gemsForPot1.First().Amount.Should().Be(100m);

				gemsForPot2.Should().HaveCount(1);
				gemsForPot2.First().Amount.Should().Be(30m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_ShouldPersistPotReference(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				Guid potId = context.PotId;
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Pot.Should().NotBeNull();
				gem.Pot.Id.Should().Be(potId);
				gem.Pot.Name.Should().Be("Referenced Pot");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithGemParameters_ShouldPersistParameters(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				Gem gem = gems.First();
				gem.Parameters.Should().HaveCount(2);
				gem.Parameters.Should().Contain("source", "mintos");
				gem.Parameters.Should().Contain("note", "monthly transfer");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithDecimalAmount_ShouldPreserveDecimalPrecision(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithAllGemCategories_ShouldPersistEachCategory(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithoutExternalId_ShouldPersistGemWithNullExternalId(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				gems.First().ExternalId.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task Add_WithExternalId_ShouldPersistExternalId(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Gem> gems = await gateway.GetAllGemsAsync();

				gems.Should().ContainSingle(x => x.ExternalId == "mintos-12345" && x.Amount == 100m);
			})
			.ExecuteAsync();
	}
}
