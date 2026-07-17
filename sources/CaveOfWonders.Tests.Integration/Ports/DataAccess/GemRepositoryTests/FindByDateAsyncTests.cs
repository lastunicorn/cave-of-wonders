using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class FindByDateAsyncTests
{
	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Gems = await repository.FindByDateAsync(Guid.NewGuid(), new DateOnly(2023, 1, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithOneGemOnThatDate_ShouldReturnThatGem(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 5, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithMultipleGemsOnSameDate_ShouldReturnAllOfThem(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithGemsOnDifferentDates_ShouldReturnOnlyGemsOnRequestedDate(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Date = new DateTime(2023, 1, 9),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot { Id = potId }
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Withdrawal,
						Amount = 30m,
						Pot = new Pot { Id = potId }
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithGemsForDifferentPotsOnSameDate_ShouldReturnOnlyGemsForRequestedPot(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Date = new DateTime(2023, 1, 10),
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
				context.Gems = await repository.FindByDateAsync(potId1, new DateOnly(2023, 1, 10))
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(100m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithNonExistentPotId_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				context.Gems = await repository.FindByDateAsync(Guid.NewGuid(), new DateOnly(2023, 1, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithNonMatchingDate_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 6, 15))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WhenGemDateHasTimeComponent_ShouldMatchOnDateOnly(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Date = new DateTime(2023, 1, 10, 18, 45, 30),
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(100m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_ShouldPopulateGemsWithMatchingPotReference(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithGemParameters_ShouldPreserveParameters(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
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
	[TestEnvironments<IGemRepository, IGemStorageGateway>]
	public async Task FindByDateAsync_WithDecimalAmount_ShouldPreserveDecimalPrecision(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				context.Gems = await repository.FindByDateAsync(potId, new DateOnly(2023, 1, 10))
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
}
