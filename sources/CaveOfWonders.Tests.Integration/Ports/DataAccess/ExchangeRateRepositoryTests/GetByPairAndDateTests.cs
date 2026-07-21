using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class GetByPairAndDateTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WhenDatabaseIsEmpty_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 10));
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
	public async Task WithExactPairAndDateMatch_ShouldReturnThatRate(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 10));
			})
			.Assert((backDoor, context) =>
			{
				ExchangeRate exchangeRate = context.ExchangeRate as ExchangeRate;

				exchangeRate.Should().NotBeNull();
				exchangeRate.CurrencyPair.Should().Be((CurrencyPair)"EURUSD");
				exchangeRate.Date.Should().Be(new DateOnly(2023, 6, 10));
				exchangeRate.Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithMatchingPairButDifferentDate_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 11));
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
	public async Task WithMatchingDateButDifferentPair_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				CurrencyPair currencyPair = "EURRON";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 10));
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
	public async Task WithOnlyInvertedPairAvailable_ShouldReturnNull(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Arrange(async (backDoor, context) =>
			{
				ExchangeRate usdEurRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "USDEUR",
					Value = 0.9259m
				};

				await backDoor.SeedExchangeRatesAsync([usdEurRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 10));
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
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.08564321m
				};

				await backDoor.SeedExchangeRatesAsync([exchangeRate]);
			})
			.Act(async (repository, context) =>
			{
				CurrencyPair currencyPair = "EURUSD";

				context.ExchangeRate = await repository.Get(currencyPair, new DateOnly(2023, 6, 10));
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