using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class GetForLatestDayAvailableTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WhenDatabaseIsEmpty_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;
				exchangeRate.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithExactDateMatch_ShouldReturnThatRate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 15),
					CurrencyPair = "EURUSD",
					Value = 1.0856m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 15));
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"EURUSD");
				exchangeRate.Value.Should().Be(1.0856m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithOnlyEarlierDateAvailable_ShouldReturnLatestAvailableRate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 10));
				exchangeRate.Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithOnlyLaterDateAvailable_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 20),
					CurrencyPair = "EURUSD",
					Value = 1.0900m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;
				exchangeRate.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithMultipleDatesAvailable_ShouldReturnTheMostRecentOneNotAfterTheGivenDate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
					Date = new DateOnly(2023, 6, 13),
					CurrencyPair = "EURUSD",
					Value = 1.0830m
				};

				ExchangeRate rate3 = new()
				{
					Date = new DateOnly(2023, 6, 20),
					CurrencyPair = "EURUSD",
					Value = 1.0900m
				};

				await backDoor.SeedExchangeRatesAsync([rate1, rate2, rate3]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 13));
				exchangeRate.Value.Should().Be(1.0830m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithRatesForOtherCurrencyPairs_ShouldIgnoreThem(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
					Date = new DateOnly(2023, 6, 14),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, eurRonRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"EURUSD");
				exchangeRate.Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithOnlyInvertedPairAvailable_AndAllowInvertedIsFalse_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate usdEurRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "USDEUR",
					Value = 0.9214m
				};

				await backDoor.SeedExchangeRatesAsync([usdEurRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15), allowInverted: false);
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;
				exchangeRate.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithOnlyInvertedPairAvailable_AndAllowInvertedIsTrue_ShouldReturnTheInvertedPairRate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate usdEurRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "USDEUR",
					Value = 0.9214m
				};

				await backDoor.SeedExchangeRatesAsync([usdEurRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15), allowInverted: true);
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"USDEUR");
				exchangeRate.Value.Should().Be(0.9214m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithBothDirectAndInvertedPairAvailable_AndAllowInvertedIsTrue_ShouldPreferTheMostRecentDateRegardlessOfDirection(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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

				ExchangeRate usdEurRate = new()
				{
					Date = new DateOnly(2023, 6, 12),
					CurrencyPair = "USDEUR",
					Value = 0.9214m
				};

				await backDoor.SeedExchangeRatesAsync([eurUsdRate, usdEurRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15), allowInverted: true);
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 12));
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"USDEUR");
				exchangeRate.Value.Should().Be(0.9214m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithNoRateAvailableForEitherDirection_AndAllowInvertedIsTrue_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate eurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await backDoor.SeedExchangeRatesAsync([eurRonRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15), allowInverted: true);
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;
				exchangeRate.Should().BeNull();
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithDecimalValue_ShouldPreserveDecimalPrecision(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 15),
					CurrencyPair = "EURUSD",
					Value = 1.08564321m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.GetForLatestDayAvailable(currencyPair, new DateOnly(2023, 6, 15));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.Value.Should().Be(1.08564321m);
			})
			.ExecuteAsync();
	}
}
