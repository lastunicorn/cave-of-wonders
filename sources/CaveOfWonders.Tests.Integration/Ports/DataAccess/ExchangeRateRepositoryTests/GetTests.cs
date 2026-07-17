using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class GetTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task Get_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
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
	public async Task Get_WithNullCurrencyPairs_ShouldReturnAllExchangeRates(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(null);
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
	public async Task Get_WithEmptyCurrencyPairsArray_ShouldReturnAllExchangeRates(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				IEnumerable<ExchangeRate> exchangeRates = await repository.Get([]);
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
	public async Task Get_WithMatchingCurrencyPair_ShouldReturnOnlyRatesForThatPair(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
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
	public async Task Get_WithMultipleCurrencyPairs_ShouldReturnRatesForAllRequestedPairs(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
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
	public async Task Get_WithNonMatchingCurrencyPair_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
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
	public async Task Get_WithMultipleDatesForSamePair_ShouldReturnAllOfThem(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
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

				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 12),
					CurrencyPair = "EURUSD",
					Value = 1.0850m
				};

				await backDoor.SeedExchangeRatesAsync([rate1, rate2, rate3]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(3);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 10) && x.Value == 1.0800m);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 11) && x.Value == 1.0830m);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 12) && x.Value == 1.0850m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task Get_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.Get(currencyPairs);
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
