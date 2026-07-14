using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class GetAllTests
{
	[Theory]
	[PotRepositoryProviders]
	public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
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
	public async Task GetAll_WithOnePot_ShouldReturnOnePot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot potInDb = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot",
					Description = "This is a test pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};
				repository.Add(potInDb);
			})
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Id.Should().NotBeEmpty();
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
	public async Task GetAll_WithMultiplePots_ShouldReturnAllPots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot pot1 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot 1",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				Pot pot2 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot 2",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 2, 1),
					Currency = "EUR"
				};

				Pot pot3 = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot 3",
					DisplayOrder = 3,
					StartDate = new DateOnly(2023, 3, 1),
					Currency = "GBP"
				};

				repository.Add(pot1);
				repository.Add(pot2);
				repository.Add(pot3);

				context.Pot1Id = pot1.Id;
				context.Pot2Id = pot2.Id;
				context.Pot3Id = pot3.Id;
			})
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(3);

				Guid expectedPot1Id = (Guid)context.Pot1Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot1Id && x.Name == "Test Pot 1");

				Guid expectedPot2Id = (Guid)context.Pot2Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot2Id && x.Name == "Test Pot 2");

				Guid expectedPot3Id = (Guid)context.Pot3Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot3Id && x.Name == "Test Pot 3");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetAll_WithPotsContainingEndDate_ShouldReturnPotsWithEndDate(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot potWithEndDate = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot with EndDate",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					EndDate = new DateOnly(2023, 12, 31),
					Currency = "USD"
				};

				Pot potWithoutEndDate = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot without EndDate",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(potWithEndDate);
				repository.Add(potWithoutEndDate);
			})
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(2);
				pots.Should().ContainSingle(x => x.Name == "Test Pot with EndDate" && x.EndDate == new DateOnly(2023, 12, 31));
				pots.Should().ContainSingle(x => x.Name == "Test Pot without EndDate" && x.EndDate == null);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetAll_WithPotsContainingSnapshots_ShouldReturnPotsWithSnapshots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot potInDb = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot with Snapshots",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				potInDb.Snapshots.AddRange([
					new PotSnapshot
					{
						Date = new DateOnly(2023, 1, 15),
						Value = 100.50m
					},
					new PotSnapshot
					{
						Date = new DateOnly(2023, 2, 15),
						Value = 120.75m
					}
				]);

				repository.Add(potInDb);
			})
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Snapshots.Should().HaveCount(2);
				pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 1, 15) && x.Value == 100.50m);
				pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 2, 15) && x.Value == 120.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Pot potInDb = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot with Labels",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				potInDb.Labels.AddRange([
					"Savings",
					"Long-term",
					"Important"
				]);

				repository.Add(potInDb);
			})
			.Act(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync().ToListAsync();
				context.Pots = pots;
			})
			.Assert((repository, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Labels.Should().HaveCount(3);
				pot.Labels.Should().Contain("Savings");
				pot.Labels.Should().Contain("Long-term");
				pot.Labels.Should().Contain("Important");
			})
			.ExecuteAsync();
	}
}