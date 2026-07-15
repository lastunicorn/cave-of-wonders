using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class FindByMonthAsyncTests
{
	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(IGemRepositorySutFixture sutFixture)
	{
		await GenericTest.Create(sutFixture)
			.Act(async (repository, context) =>
			{
				context.Gems = await repository.FindByMonthAsync(Guid.NewGuid(), new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithOneGemInThatMonth_ShouldReturnThatGem(IGemRepositorySutFixture sutFixture)
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
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 5))
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithMultipleGemsInSameMonth_ShouldReturnAllOfThem(IGemRepositorySutFixture sutFixture)
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
					Date = new DateTime(2023, 1, 5),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 15),
					Category = GemCategory.Withdrawal,
					Amount = 50m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 25),
					Category = GemCategory.Gain,
					Amount = 25m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithGemsInDifferentMonths_ShouldReturnOnlyGemsInRequestedMonth(IGemRepositorySutFixture sutFixture)
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
					Date = new DateTime(2022, 12, 31),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 15),
					Category = GemCategory.Deposit,
					Amount = 200m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 2, 1),
					Category = GemCategory.Withdrawal,
					Amount = 30m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithSameMonthNumberInDifferentYear_ShouldReturnOnlyGemsInRequestedYear(IGemRepositorySutFixture sutFixture)
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
					StartDate = new DateOnly(2022, 1, 1),
					Currency = "USD"
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2022, 3, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 3, 10),
					Category = GemCategory.Deposit,
					Amount = 200m,
					Pot = new Pot { Id = potId }
				});

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 3))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(200m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithGemsForDifferentPotsInSameMonth_ShouldReturnOnlyGemsForRequestedPot(IGemRepositorySutFixture sutFixture)
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
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId1 }
				});

				repository.Add(new Gem
				{
					Id = Guid.NewGuid(),
					Date = new DateTime(2023, 1, 20),
					Category = GemCategory.Withdrawal,
					Amount = 30m,
					Pot = new Pot { Id = potId2 }
				});

				context.PotId1 = potId1;
			})
			.Act(async (repository, context) =>
			{
				Guid potId1 = context.PotId1;
				context.Gems = await repository.FindByMonthAsync(potId1, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(100m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithNonExistentPotId_ShouldReturnEmptyCollection(IGemRepositorySutFixture sutFixture)
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
					Date = new DateTime(2023, 1, 10),
					Category = GemCategory.Deposit,
					Amount = 100m,
					Pot = new Pot { Id = potId }
				});
			})
			.Act(async (repository, context) =>
			{
				context.Gems = await repository.FindByMonthAsync(Guid.NewGuid(), new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithNonMatchingMonth_ShouldReturnEmptyCollection(IGemRepositorySutFixture sutFixture)
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
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 6))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;
				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_ShouldPopulateGemsWithMatchingPotReference(IGemRepositorySutFixture sutFixture)
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
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithGemParameters_ShouldPreserveParameters(IGemRepositorySutFixture sutFixture)
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
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
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
	[GemRepositoryProviders]
	public async Task FindByMonthAsync_WithDecimalAmount_ShouldPreserveDecimalPrecision(IGemRepositorySutFixture sutFixture)
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
				context.Gems = await repository.FindByMonthAsync(potId, new MonthDate(2023, 1))
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Gem> gems = context.Gems as List<Gem>;

				gems.Should().HaveCount(1);
				gems.First().Amount.Should().Be(123.456789m);
			})
			.ExecuteAsync();
	}
}
