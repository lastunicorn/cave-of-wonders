using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.GemRepositoryTests;

public class RemoveTests
{
	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_WithExistingGem_ShouldDeleteGem(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();
				Guid gemId = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId,
						Date = new DateTime(2023, 5, 10),
						Category = GemCategory.Deposit,
						Amount = 150.75m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.GemId = gemId;
			})
			.Act((repository, context) =>
			{
				Guid gemId = context.GemId;

				repository.Remove(new Gem
				{
					Id = gemId
				});
			})
			.Assert(async (backDoor, context) =>
			{
				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_WithGemInstanceRetrievedFromRepository_ShouldDeleteGem(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();
				Guid gemId = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId,
						Date = new DateTime(2023, 5, 10),
						Category = GemCategory.Deposit,
						Amount = 150.75m,
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

				List<Gem> gems = await repository.GetByPotIdAsync(potId)
					.ToListAsync();
				Gem gem = gems.Single();

				repository.Remove(gem);
			})
			.Assert(async (backDoor, context) =>
			{
				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_WithNullGem_ShouldThrowArgumentNullException(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
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
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_WithMultipleGemsForSamePot_ShouldOnlyRemoveTargetGem(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();
				Guid gemId1 = Guid.NewGuid();
				Guid gemId2 = Guid.NewGuid();
				Guid gemId3 = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId1,
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 10m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = gemId2,
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 20m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					new Gem
					{
						Id = gemId3,
						Date = new DateTime(2023, 3, 10),
						Category = GemCategory.Gain,
						Amount = 30m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.GemId1 = gemId1;
				context.GemId2 = gemId2;
				context.GemId3 = gemId3;
			})
			.Act((repository, context) =>
			{
				Guid gemId2 = context.GemId2;

				repository.Remove(new Gem
				{
					Id = gemId2
				});
			})
			.Assert(async (backDoor, context) =>
			{
				Guid gemId1 = context.GemId1;
				Guid gemId2 = context.GemId2;
				Guid gemId3 = context.GemId3;

				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().HaveCount(2);
				gems.Select(x => x.Id).Should().BeEquivalentTo([gemId1, gemId3]);
				gems.Select(x => x.Id).Should().NotContain(gemId2);
				gems.Should().ContainSingle(x => x.Id == gemId1 && x.Amount == 10m);
				gems.Should().ContainSingle(x => x.Id == gemId3 && x.Amount == 30m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_WithGemsAcrossDifferentPots_ShouldNotAffectOtherPotsGems(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId1 = Guid.NewGuid();
				Guid potId2 = Guid.NewGuid();
				Guid gemId1 = Guid.NewGuid();
				Guid gemId2 = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId1,
					Name = "Pot 1",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId2,
					Name = "Pot 2",
					DisplayOrder = 2,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "EUR"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId1,
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
						Id = gemId2,
						Date = new DateTime(2023, 1, 11),
						Category = GemCategory.Withdrawal,
						Amount = 30m,
						Pot = new Pot
						{
							Id = potId2
						}
					}
				]);

				context.PotId2 = potId2;
				context.GemId1 = gemId1;
				context.GemId2 = gemId2;
			})
			.Act((repository, context) =>
			{
				Guid gemId1 = context.GemId1;

				repository.Remove(new Gem
				{
					Id = gemId1
				});
			})
			.Assert(async (backDoor, context) =>
			{
				Guid potId2 = context.PotId2;
				Guid gemId2 = context.GemId2;

				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				Gem remainingGem = gems.Single();
				remainingGem.Id.Should().Be(gemId2);
				remainingGem.Pot.Id.Should().Be(potId2);
				remainingGem.Amount.Should().Be(30m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_AllGemsForPot_ShouldLeaveNoGemsBehind(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();
				Guid gemId1 = Guid.NewGuid();
				Guid gemId2 = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId1,
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
						Id = gemId2,
						Date = new DateTime(2023, 2, 10),
						Category = GemCategory.Withdrawal,
						Amount = 40m,
						Pot = new Pot
						{
							Id = potId
						}
					}
				]);

				context.GemId1 = gemId1;
				context.GemId2 = gemId2;
			})
			.Act((repository, context) =>
			{
				Guid gemId1 = context.GemId1;
				Guid gemId2 = context.GemId2;

				repository.Remove(new Gem
				{
					Id = gemId1
				});
				repository.Remove(new Gem
				{
					Id = gemId2
				});
			})
			.Assert(async (backDoor, context) =>
			{
				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IGemRepository, ITestBackDoor>]
	public async Task Remove_ShouldPreserveParametersOfRemainingGems(ITestEnvironment<IGemRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				Guid potId = Guid.NewGuid();
				Guid gemId1 = Guid.NewGuid();
				Guid gemId2 = Guid.NewGuid();

				await backDoor.SeedPotAsync(new Pot
				{
					Id = potId,
					Name = "Test Pot",
					DisplayOrder = 1,
					StartDate = new DateOnly(2023, 1, 1),
					Currency = "USD"
				});

				Gem gemToKeep = new()
				{
					Id = gemId2,
					Date = new DateTime(2023, 2, 10),
					Category = GemCategory.Withdrawal,
					Amount = 40m,
					Pot = new Pot
					{
						Id = potId
					}
				};
				gemToKeep.Parameters.Add(new GemParameter
				{
					Key = "source",
					Value = "mintos"
				});

				await backDoor.SeedGemsAsync([
					new Gem
					{
						Id = gemId1,
						Date = new DateTime(2023, 1, 10),
						Category = GemCategory.Deposit,
						Amount = 100m,
						Pot = new Pot
						{
							Id = potId
						}
					},
					gemToKeep
				]);

				context.GemId1 = gemId1;
				context.GemId2 = gemId2;
			})
			.Act((repository, context) =>
			{
				Guid gemId1 = context.GemId1;

				repository.Remove(new Gem
				{
					Id = gemId1
				});
			})
			.Assert(async (backDoor, context) =>
			{
				Guid gemId2 = context.GemId2;

				List<Gem> gems = await backDoor.GetAllGemsAsync();

				gems.Should().HaveCount(1);
				Gem remainingGem = gems.Single();
				remainingGem.Id.Should().Be(gemId2);
				remainingGem.Parameters.Should().ContainSingle();
				remainingGem.Parameters.Should().Contain(x => x.Key == "source" && x.Value == "mintos");
			})
			.ExecuteAsync();
	}
}