using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.Gateways;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class FindAsyncTests
{
	[Theory]
	[GemRepositoryEnvironments]
	public async Task FindAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					PotId = Guid.NewGuid()
				};
				context.Gems = await repository.FindAsync(filter)
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
	public async Task FindAsync_WithEmptyFilter_ShouldReturnAllGems(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId1
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot
						{
							Id = potId2
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new();
				context.Gems = await repository.FindAsync(filter)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().HaveCount(2);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithPotIdFilter_ShouldReturnOnlyGemsForThatPot(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId1
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot
						{
							Id = potId1
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 30m,
						Pot = new Pot
						{
							Id = potId2
						}
					}
				]);

				context.PotId1 = potId1;
			})
			.Act(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				GemFilter filter = new()
				{
					PotId = potId1
				};
				context.Gems = await repository.FindAsync(filter)
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
	public async Task FindAsync_WithDateFilter_ShouldReturnOnlyGemsOnThatDate(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					Date = new DateOnly(2023, 1, 10)
				};
				context.Gems = await repository.FindAsync(filter)
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
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithMonthFilter_ShouldReturnOnlyGemsInThatMonth(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 25),
						Category = GemCategory.Withdrawal,
						Amount = 40m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 2, 5),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					Month = new MonthDate(2023, 1)
				};
				context.Gems = await repository.FindAsync(filter)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(2);
				gems.Should().OnlyContain(x => x.Amount == 100m || x.Amount == 40m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithCategoriesFilter_ShouldReturnOnlyGemsWithMatchingCategories(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 12),
						Category = GemCategory.Bonus,
						Amount = 25m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					Categories = [GemCategory.Deposit, GemCategory.Bonus]
				};
				context.Gems = await repository.FindAsync(filter)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(2);
				gems.Should().OnlyContain(x => x.Category == GemCategory.Deposit || x.Category == GemCategory.Bonus);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithExternalIdFilter_ShouldReturnOnlyGemWithMatchingExternalId(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Deposit,
						Amount = 200m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					ExternalId = "ext-2"
				};
				context.Gems = await repository.FindAsync(filter)
					.ToListAsync();
			})
			.Assert((gateway, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().ExternalId.Should().Be("ext-2");
				gems.First().Amount.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithCombinedPotIdAndCategoryFilters_ShouldReturnGemsMatchingAllCriteria(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId1
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Withdrawal,
						Amount = 50m,
						Pot = new Pot
						{
							Id = potId1
						}
					},
					new Gem
					{
						Id = Guid.NewGuid(),
						Date = new DateTime(2023, 1, 12),
						Category = GemCategory.Deposit,
						Amount = 300m,
						Pot = new Pot
						{
							Id = potId2
						}
					}
				]);

				context.PotId1 = potId1;
			})
			.Act(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				GemFilter filter = new()
				{
					PotId = potId1,
					Categories = [GemCategory.Deposit]
				};
				context.Gems = await repository.FindAsync(filter)
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
	[GemRepositoryEnvironments]
	public async Task FindAsync_WithNonMatchingFilter_ShouldReturnEmptyCollection(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);
			})
			.Act(async (repository, context) =>
			{
				GemFilter filter = new()
				{
					Categories = [GemCategory.Withdrawal]
				};
				context.Gems = await repository.FindAsync(filter)
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
	public async Task FindAsync_ShouldPopulateGemsWithMatchingPotReference(ITestEnvironment<IGemRepository, IGemStorageGateway> environment)
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
				GemFilter filter = new()
				{
					PotId = potId
				};
				context.Gems = await repository.FindAsync(filter)
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
}
