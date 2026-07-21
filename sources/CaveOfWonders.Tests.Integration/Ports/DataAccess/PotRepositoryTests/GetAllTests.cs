using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class GetAllTests
{
	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;
				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WithOnePot_ShouldReturnOnePot(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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
				await backDoor.SeedPotsAsync([potInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
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
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WithMultiplePots_ShouldReturnAllPots(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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

				await backDoor.SeedPotsAsync([pot1, pot2, pot3]);

				context.Pot1Id = pot1.Id;
				context.Pot2Id = pot2.Id;
				context.Pot3Id = pot3.Id;
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
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
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WithPotsContainingEndDate_ShouldReturnPotsWithEndDate(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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

				await backDoor.SeedPotsAsync([potWithEndDate, potWithoutEndDate]);
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(2);
				pots.Should().ContainSingle(x => x.Name == "Test Pot with EndDate" && x.EndDate == new DateOnly(2023, 12, 31));
				pots.Should().ContainSingle(x => x.Name == "Test Pot without EndDate" && x.EndDate == null);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WithPotsContainingSnapshots_ShouldReturnPotsWithSnapshots(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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

				await backDoor.SeedPotsAsync([potInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
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
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task GetAll_WithPotsContainingLabels_ShouldReturnPotsWithLabels(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
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
					new PotLabel
					{
						Label = "Savings"
					},
					new PotLabel
					{
						Label = "Long-term"
					},
					new PotLabel
					{
						Label = "Important"
					}
				]);

				await backDoor.SeedPotsAsync([potInDb]);
			})
			.Act(async (repository, context) =>
			{
				context.Pots = await repository.GetAllAsync()
					.ToListAsync();
			})
			.Assert((backDoor, context) =>
			{
				List<Pot> pots = context.Pots as List<Pot>;

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Labels.Should().HaveCount(3);
				pot.Labels.Should().Contain(x => x.Label == "Savings");
				pot.Labels.Should().Contain(x => x.Label == "Long-term");
				pot.Labels.Should().Contain(x => x.Label == "Important");
			})
			.ExecuteAsync();
	}
}