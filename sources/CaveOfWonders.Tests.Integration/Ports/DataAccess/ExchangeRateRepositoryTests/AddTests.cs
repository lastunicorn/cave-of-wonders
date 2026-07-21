using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class AddTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithNewItem_ShouldInsertIt(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				repository.Add(exchangeRate);
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(1);
				ExchangeRate exchangeRate = exchangeRates.First();
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"EURUSD");
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 10));
				exchangeRate.Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithTwoNewItems_ShouldInsertBoth(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act((repository, context) =>
			{
				repository.Add(new ExchangeRate
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				});

				repository.Add(new ExchangeRate
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				});
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURUSD" && x.Value == 1.0800m);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURRON" && x.Value == 4.9700m);
			})
			.ExecuteAsync();
	}

	// Updating an exchange rate is no longer a repository-level concern: the caller fetches the
	// existing rate via Get, mutates it directly, then the session is saved (in production, via
	// IUnitOfWork.SaveChangesAsync; here, implicitly when the SUT session closes after Act). These
	// two tests guard the adapter-specific plumbing (LiteDb/SQLite trackers, Json's live in-memory
	// list) that makes that in-place mutation actually observable after a save.
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithMutatedExistingItem_ShouldPersistUpdatedValueWithoutDuplicating(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate eurUsdRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate]);
			})
			.Act(async (repository, context) =>
			{
				ExchangeRate existing = await repository.Get((CurrencyPair)"EURUSD", new DateOnly(2023, 6, 10));
				existing.Value = 1.0900m;
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(1);
				exchangeRates.First().Value.Should().Be(1.0900m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithExistingItemFetchedButNotMutated_ShouldLeaveItUnchanged(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate eurUsdRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate]);
			})
			.Act(async (repository, context) =>
			{
				await repository.Get((CurrencyPair)"EURUSD", new DateOnly(2023, 6, 10));
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(1);
				exchangeRates.First().Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}
}