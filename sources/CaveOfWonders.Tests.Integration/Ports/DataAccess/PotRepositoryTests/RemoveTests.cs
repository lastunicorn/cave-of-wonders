using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.PotRepositoryTests;

public class RemoveTests
{
	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_WithExistingPot_ShouldDeletePot(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				await backDoor.SeedPotsAsync([
					new Pot
					{
						Id = potId,
						Name = "Test Pot",
						DisplayOrder = 1,
						StartDate = new DateOnly(2023, 1, 1),
						Currency = "USD"
					}
				]);

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Remove(new Pot
				{
					Id = potId
				});
			})
			.Assert(async (backDoor, context) =>
			{
				List<Pot> pots = await backDoor.GetAllPotsAsync();

				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_WithPotInstanceRetrievedFromRepository_ShouldDeletePot(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				await backDoor.SeedPotsAsync([
					new Pot
					{
						Id = potId,
						Name = "Test Pot",
						DisplayOrder = 1,
						StartDate = new DateOnly(2023, 1, 1),
						Currency = "USD"
					}
				]);

				context.PotId = potId;
			})
			.Act(async (repository, context) =>
			{
				Guid potId = context.PotId;

				List<Pot> matchingPots = await repository.GetAsync(new PotFlexId(potId))
					.ToListAsync();
				Pot pot = matchingPots.Single();

				repository.Remove(pot);
			})
			.Assert(async (backDoor, context) =>
			{
				List<Pot> pots = await backDoor.GetAllPotsAsync();

				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_WithNullPot_ShouldThrowArgumentNullException(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.Remove(null);
			})
			.AssertThrow(ex =>
			{
				ex.Should().BeOfType<ArgumentNullException>();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_WithMultiplePots_ShouldOnlyRemoveTargetPot(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();
				Guid potId3 = Guid.NewGuid();

				await backDoor.SeedPotsAsync([
					new Pot
					{
						Id = potId1,
						Name = "Pot 1",
						DisplayOrder = 1,
						StartDate = new DateOnly(2023, 1, 1),
						Currency = "USD"
					},
					new Pot
					{
						Id = potId2,
						Name = "Pot 2",
						DisplayOrder = 2,
						StartDate = new DateOnly(2023, 1, 1),
						Currency = "EUR"
					},
					new Pot
					{
						Id = potId3,
						Name = "Pot 3",
						DisplayOrder = 3,
						StartDate = new DateOnly(2023, 1, 1),
						Currency = "GBP"
					}
				]);

				context.PotId1 = potId1;
				context.PotId2 = potId2;
				context.PotId3 = potId3;
			})
			.Act((repository, context) =>
			{
				Guid potId2 = context.PotId2;

				repository.Remove(new Pot
				{
					Id = potId2
				});
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId1 = context.PotId1;
				Guid potId2 = context.PotId2;
				Guid potId3 = context.PotId3;

				List<Pot> pots = await backDoor.GetAllPotsAsync();

				pots.Should().HaveCount(2);
				pots.Select(x => x.Id).Should().BeEquivalentTo([potId1, potId3]);
				pots.Select(x => x.Id).Should().NotContain(potId2);
				pots.Should().ContainSingle(x => x.Id == potId1 && x.Name == "Pot 1");
				pots.Should().ContainSingle(x => x.Id == potId3 && x.Name == "Pot 3");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_PotWithSnapshotsAndLabels_ShouldRemoveEverything(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();

				Pot pot = new()
				{
					Id = potId,
					Name = "Test Pot with Snapshots and Labels",
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

				pot.Labels.AddRange([
					new PotLabel { Label = "Savings" },
					new PotLabel { Label = "Long-term" }
				]);

				await backDoor.SeedPotsAsync([pot]);

				context.PotId = potId;
			})
			.Act((repository, context) =>
			{
				Guid potId = context.PotId;

				repository.Remove(new Pot
				{
					Id = potId
				});
			})
			.Assert(async (backDoor, context) =>
			{
				List<Pot> pots = await backDoor.GetAllPotsAsync();

				pots.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IPotRepository, ITestBackDoor>]
	public async Task Remove_ShouldNotAffectOtherPotsSnapshotsAndLabels(ITestEnvironment<IPotRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();

				Pot pot1 = new()
				{
					Id = potId1,
					Name = "Pot To Remove",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				};
				pot1.Snapshots.Add(new PotSnapshot
				{
					Date = new DateOnly(2023, 1, 15),
					Value = 50m
				});
				pot1.Labels.Add(new PotLabel { Label = "Temporary" });

				Pot pot2 = new()
				{
					Id = potId2,
					Name = "Pot To Keep",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "EUR"
				};
				pot2.Snapshots.Add(new PotSnapshot
				{
					Date = new DateOnly(2023, 2, 20),
					Value = 200m
				});
				pot2.Labels.AddRange([
					new PotLabel { Label = "Savings" },
					new PotLabel { Label = "Important" }
				]);

				await backDoor.SeedPotsAsync([pot1, pot2]);

				context.PotId1 = potId1;
				context.PotId2 = potId2;
			})
			.Act((repository, context) =>
			{
				Guid potId1 = context.PotId1;

				repository.Remove(new Pot
				{
					Id = potId1
				});
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId2 = context.PotId2;

				List<Pot> pots = await backDoor.GetAllPotsAsync();

				pots.Should().HaveCount(1);
				Pot remainingPot = pots.Single();
				remainingPot.Id.Should().Be(potId2);
				remainingPot.Name.Should().Be("Pot To Keep");
				remainingPot.Snapshots.Should().ContainSingle(x => x.Date == new DateOnly(2023, 2, 20) && x.Value == 200m);
				remainingPot.Labels.Should().HaveCount(2);
				remainingPot.Labels.Should().Contain(x => x.Label == "Savings");
				remainingPot.Labels.Should().Contain(x => x.Label == "Important");
			})
			.ExecuteAsync();
	}
}