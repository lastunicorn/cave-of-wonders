using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class GetByDateIntervalTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;
				exchangeRates.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithNullCurrencyPairsAndNoDateBounds_ShouldReturnAllExchangeRates(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				ExchangeRate eurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, eurRonRate]);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(null, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURUSD" && x.Value == 1.0800m);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURRON" && x.Value == 4.9700m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithEmptyCurrencyPairsArray_ShouldReturnAllExchangeRates(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				ExchangeRate eurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, eurRonRate]);
			})
			.Act(async (repository, context) =>
			{
				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval([], null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;
				exchangeRates.Should().HaveCount(2);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithMatchingCurrencyPair_ShouldReturnOnlyRatesForThatPair(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				ExchangeRate eurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, eurRonRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

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
	public async Task GetByDateInterval_WithMultipleCurrencyPairs_ShouldReturnRatesForAllRequestedPairs(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				ExchangeRate eurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				ExchangeRate usdRonRate = new()
				{
					Date = new DateOnly(2023, 6, 12),
					CurrencyPair = "USDRON",
					Value = 4.6000m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, eurRonRate, usdRonRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD", "EURRON"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURUSD");
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURRON");
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithNonMatchingCurrencyPair_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				CurrencyPair[] currencyPairs = ["EURRON"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;
				exchangeRates.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithStartDateOnly_ShouldReturnRatesOnOrAfterStartDate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate1 = new()
				{
					Date = new DateOnly(2023, 6, 9),
					CurrencyPair = "EURUSD",
					Value = 1.0790m
				};

				ExchangeRate rate2 = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURUSD",
					Value = 1.0830m
				};

				await backDoor.SeedExchangeRatesAsync([rate1, rate2, rate3]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, new DateOnly(2023, 6, 10), null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 10));
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 11));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithEndDateOnly_ShouldReturnRatesOnOrBeforeEndDate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate1 = new()
				{
					Date = new DateOnly(2023, 6, 9),
					CurrencyPair = "EURUSD",
					Value = 1.0790m
				};

				ExchangeRate rate2 = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURUSD",
					Value = 1.0830m
				};

				await backDoor.SeedExchangeRatesAsync([rate1, rate2, rate3]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, new DateOnly(2023, 6, 10));
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 9));
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 10));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithStartAndEndDate_ShouldReturnRatesWithinInclusiveRange(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate1 = new()
				{
					Date = new DateOnly(2023, 6, 9),
					CurrencyPair = "EURUSD",
					Value = 1.0790m
				};

				ExchangeRate rate2 = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURUSD",
					Value = 1.0830m
				};

				ExchangeRate rate4 = new()
				{
					Date = new DateOnly(2023, 6, 12),
					CurrencyPair = "EURUSD",
					Value = 1.0850m
				};

				await backDoor.SeedExchangeRatesAsync([rate1, rate2, rate3, rate4]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, new DateOnly(2023, 6, 10), new DateOnly(2023, 6, 11));
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 10) && x.Value == 1.0800m);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 11) && x.Value == 1.0830m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithNoRatesInRange_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([rate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, new DateOnly(2023, 7, 1), new DateOnly(2023, 7, 31));
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;
				exchangeRates.Should().BeEmpty();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithRatesAddedOutOfOrder_ShouldReturnThemOrderedByDateAscending(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 12),
					CurrencyPair = "EURUSD",
					Value = 1.0850m
				};

				ExchangeRate rate1 = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate rate2 = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURUSD",
					Value = 1.0830m
				};

				await backDoor.SeedExchangeRatesAsync([rate3, rate1, rate2]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(3);
				exchangeRates.Should().BeInAscendingOrder(x => x.Date);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByDateInterval_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.08564321m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByDateInterval(currencyPairs, null, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(1);
				exchangeRates.First().Value.Should().Be(1.08564321m);
			})
			.ExecuteAsync();
	}
}