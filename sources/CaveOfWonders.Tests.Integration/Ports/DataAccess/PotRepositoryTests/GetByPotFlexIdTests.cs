using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class GetByPotFlexIdTests
{
	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = Guid.NewGuid();

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;
				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithMatchingGuid_ShouldReturnMatchingPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					Description = "This is a test pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				PotFlexId potFlexId = potId;

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);
				Pot pot = pots.First();

				Guid expectedPotId = context.PotId;
				pot.Id.Should().Be(expectedPotId);
				pot.Name.Should().Be("Test Pot");
				pot.Description.Should().Be("This is a test pot");
				pot.DisplayOrder.Should().Be(1);
				pot.StartDate.Should().Be(new DateOnly(2023, 1, 1));
				pot.Currency.Should().Be("USD");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithNonMatchingGuid_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = Guid.NewGuid();

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;
				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithPartialGuid_ShouldReturnMatchingPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;
				string partialGuid = potId.ToString("N")[..8];
				PotFlexId potFlexId = partialGuid;

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				pots.First().Id.Should().Be(expectedPotId);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithExactNameMatch_ShouldReturnMatchingPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Emergency Fund",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = "Emergency Fund";

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				pots.First().Id.Should().Be(expectedPotId);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithPartialNameMatch_ShouldReturnMatchingPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Emergency Fund",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = "mergency";

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				pots.First().Id.Should().Be(expectedPotId);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithNameMatchDifferentCase_ShouldReturnMatchingPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Savings Account",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = pot.Id;
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = "SAVINGS";

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);

				Guid expectedPotId = context.PotId;
				pots.First().Id.Should().Be(expectedPotId);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithNonMatchingName_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Emergency Fund",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = "NoSuchPot";

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;
				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WithMultipleMatchingPots_ShouldReturnAllMatches(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot savingsPot1 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Savings Account A",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				Pot savingsPot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Savings Account B",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				Pot checkingPot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Checking Account",
					DisplayOrder = 3,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(savingsPot1);
				repository.Add(savingsPot2);
				repository.Add(checkingPot);

				context.SavingsPot1Id = savingsPot1.Id;
				context.SavingsPot2Id = savingsPot2.Id;
			})
			.Act(async (repository, context) =>
			{
				PotFlexId potFlexId = "Savings";

				context.Pots = await repository.GetAsync(potFlexId)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(2);

				Guid expectedSavingsPot1Id = context.SavingsPot1Id;
				pots.Should().ContainSingle(x => x.Id == expectedSavingsPot1Id);

				Guid expectedSavingsPot2Id = context.SavingsPot2Id;
				pots.Should().ContainSingle(x => x.Id == expectedSavingsPot2Id);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetByPotFlexId_WhenPotFlexIdIsEmpty_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAsync(PotFlexId.Empty)
					.ToListAsync();
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;
				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}
}