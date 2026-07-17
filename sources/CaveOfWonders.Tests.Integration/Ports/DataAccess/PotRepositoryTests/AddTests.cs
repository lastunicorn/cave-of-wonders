using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class AddTests
{
	[Theory]
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithValidPot_ShouldPersistPot(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

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
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithNullPot_ShouldThrowArgumentNullException(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
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
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithMultiplePots_ShouldPersistAllPots(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

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
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithPotContainingSnapshots_ShouldPersistSnapshots(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

				pots.Should().HaveCount(1);
				Pot pot = pots.First();
				pot.Snapshots.Should().HaveCount(2);
				pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 1, 15) && x.Value == 100.50m);
				pot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 2, 15) && x.Value == 120.75m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithPotContainingLabels_ShouldPersistLabels(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

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
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithPotContainingEndDate_ShouldPersistEndDate(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

				pots.Should().HaveCount(2);
				pots.Should().ContainSingle(x => x.Name == "Test Pot with EndDate" && x.EndDate == new DateOnly(2023, 12, 31));
				pots.Should().ContainSingle(x => x.Name == "Test Pot without EndDate" && x.EndDate == null);
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithNullDescription_ShouldPersistNullDescription(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

				pots.Should().HaveCount(1);
				pots.First().Description.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithNoSnapshotsOrLabels_ShouldPersistEmptyCollections(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		await GenericTest.Create(environment)
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
			.Assert(async (gateway, context) =>
			{
				List<Pot> pots = await gateway.GetAllPotsAsync();

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
	[RepositoryEnvironments<IPotRepository, IPotStorageGateway>]
	public async Task Add_WithDuplicateId_ShouldThrow(ITestEnvironment<IPotRepository, IPotStorageGateway> environment)
	{
		// Some adapters (e.g. the EF Core-backed SQLite one) only stage the entity in Add and
		// don't hit the database - and therefore don't surface a duplicate-key violation -
		// until changes are flushed, which for this test happens inside the environment's own
		// ReleaseSutAsync rather than inside the Act delegate. AssertThrow captures the
		// exception from the whole Act phase, so the test passes regardless of whether the
		// given adapter rejects the duplicate synchronously or on flush.
		await GenericTest.Create(environment)
			.Arrange(async (gateway, context) =>
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

				await gateway.SeedPotsAsync([pot]);
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

				repository.Add(duplicatePot);
			})
			.AssertThrow(ex =>
			{
				ex.Should().NotBeNull();
			})
			.ExecuteAsync();
	}
}
