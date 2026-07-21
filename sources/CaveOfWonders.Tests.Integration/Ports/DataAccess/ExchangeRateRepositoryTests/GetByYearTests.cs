using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class GetByYearTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByYear_WhenDatabaseIsEmpty_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithNullCurrencyPairsAndNoMonth_ShouldReturnAllRatesForYear(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(null, 2023, null);
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
	public async Task GetByYear_WithEmptyCurrencyPairsArray_ShouldReturnAllRatesForYear(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear([], 2023, null);
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
	public async Task GetByYear_WithMatchingCurrencyPair_ShouldReturnOnlyRatesForThatPair(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithMultipleCurrencyPairs_ShouldReturnRatesForAllRequestedPairs(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithNonMatchingCurrencyPair_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithRatesInDifferentYears_ShouldReturnOnlyMatchingYear(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate2022 = new()
				{
					Date = new DateOnly(2022, 12, 31),
					CurrencyPair = "EURUSD",
					Value = 1.0700m
				};

				ExchangeRate rate2023 = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate rate2024 = new()
				{
					Date = new DateOnly(2024, 1, 1),
					CurrencyPair = "EURUSD",
					Value = 1.0900m
				};

				await backDoor.SeedExchangeRatesAsync([rate2022, rate2023, rate2024]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(1);
				exchangeRates.First().Date.Should().Be(new DateOnly(2023, 6, 10));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByYear_WithNoMatchingYear_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate rate = new()
				{
					Date = new DateOnly(2022, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0700m
				};

				await backDoor.SeedExchangeRatesAsync([rate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithMonthSpecified_ShouldReturnOnlyRatesForThatMonth(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate mayRate = new()
				{
					Date = new DateOnly(2023, 5, 20),
					CurrencyPair = "EURUSD",
					Value = 1.0750m
				};

				ExchangeRate juneRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				ExchangeRate julyRate = new()
				{
					Date = new DateOnly(2023, 7, 5),
					CurrencyPair = "EURUSD",
					Value = 1.0850m
				};

				await backDoor.SeedExchangeRatesAsync([mayRate, juneRate, julyRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, 6);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(1);
				ExchangeRate exchangeRate = exchangeRates.First();
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 10));
				exchangeRate.Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByYear_WithMonthNull_ShouldReturnRatesForAllMonthsInYear(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate mayRate = new()
				{
					Date = new DateOnly(2023, 5, 20),
					CurrencyPair = "EURUSD",
					Value = 1.0750m
				};

				ExchangeRate juneRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([mayRate, juneRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
				context.ExchangeRates = exchangeRates.ToList();
			})
			.Assert((backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = context.ExchangeRates as List<ExchangeRate>;

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 5, 20));
				exchangeRates.Should().ContainSingle(x => x.Date == new DateOnly(2023, 6, 10));
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task GetByYear_WithNoMatchingMonth_ShouldReturnEmptyCollection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate juneRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([juneRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair[] currencyPairs = ["EURUSD"];

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, 7);
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
	public async Task GetByYear_WithRatesAddedOutOfOrder_ShouldReturnThemOrderedByDateAscending(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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
	public async Task GetByYear_WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				IEnumerable<ExchangeRate> exchangeRates = await repository.GetByYear(currencyPairs, 2023, null);
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