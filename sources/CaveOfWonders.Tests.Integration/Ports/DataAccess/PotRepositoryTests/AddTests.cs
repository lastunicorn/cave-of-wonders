using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.SutFixtures;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class AddTests
{
	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithValidPot_ShouldPersistPot(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
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
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

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
	public async Task Add_WithNullPot_ShouldThrowArgumentNullException(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
			{
				try
				{
					repository.Add(null);
					context.Exception = null;
				}
				catch (Exception ex)
				{
					context.Exception = ex;
				}
			})
			.Assert((repository, context) =>
			{
				Exception exception = context.Exception;
				exception.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithMultiplePots_ShouldPersistAllPots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
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
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

				pots.Should().HaveCount(3);

				Guid expectedPot1Id = context.Pot1Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot1Id && x.Name == "Test Pot 1");

				Guid expectedPot2Id = context.Pot2Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot2Id && x.Name == "Test Pot 2");

				Guid expectedPot3Id = context.Pot3Id;
				pots.Should().ContainSingle(x => x.Id == expectedPot3Id && x.Name == "Test Pot 3");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithPotContainingSnapshots_ShouldPersistSnapshots(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot with Snapshots",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				pot.Snapshots.AddRange([
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

				repository.Add(pot);
			})
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

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
	public async Task Add_WithPotContainingLabels_ShouldPersistLabels(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot with Labels",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				pot.Labels.AddRange([
					"Savings",
					"Long-term",
					"Important"
				]);

				repository.Add(pot);
			})
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Labels.Should().HaveCount(3);
				pot.Labels.Should().Contain("Savings");
				pot.Labels.Should().Contain("Long-term");
				pot.Labels.Should().Contain("Important");
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithPotContainingEndDate_ShouldPersistEndDate(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
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
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

				pots.Should().HaveCount(2);
				pots.Should().ContainSingle(x => x.Name == "Test Pot with EndDate" && x.EndDate == new DateOnly(2023, 12, 31));
				pots.Should().ContainSingle(x => x.Name == "Test Pot without EndDate" && x.EndDate == null);
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithNullDescription_ShouldPersistNullDescription(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Test Pot without Description",
					Description = null,
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

				pots.Should().HaveCount(1);
				pots.First().Description.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithNoSnapshotsOrLabels_ShouldPersistEmptyCollections(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Act((repository, context) =>
			{
				Pot pot = new()
				{
					Id = Guid.NewGuid(),
					Name = "Bare Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
			})
			.Assert(async (repository, context) =>
			{
				List<Pot> pots = await repository.GetAllAsync()
					.ToListAsync();

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Snapshots.Should().NotBeNull();
				pot.Snapshots.Should().BeEmpty();
				pot.Labels.Should().NotBeNull();
				pot.Labels.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[PotRepositoryProviders]
	public async Task Add_WithDuplicateId_ShouldThrow(ISutFixture<IPotRepository> sutFixture)
	{
		await new GenericTest<IPotRepository>(sutFixture)
			.Arrange((repository, context) =>
			{
				Guid potId = Guid.NewGuid();

				Pot pot = new()
				{
					Id = potId,
					Name = "Original Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				repository.Add(pot);
				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				Pot duplicatePot = new()
				{
					Id = potId,
					Name = "Duplicate Pot",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};

				try
				{
					repository.Add(duplicatePot);
					context.Exception = null;
				}
				catch (Exception ex)
				{
					context.Exception = ex;
				}
			})
			.Assert((repository, context) =>
			{
				Exception exception = context.Exception;
				exception.Should().NotBeNull();
			})
			.ExecuteAsync();
	}
}
