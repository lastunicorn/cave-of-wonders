using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests.TestEnvironments;
using DustInTheWind.CaveOfWonders.Tests.Utils;
using FluentAssertions;

namespace DustInTheWind.CaveOfWonders.Tests.Integration.Ports.DataAccess.ExchangeRateRepositoryTests;

public class AddOrUpdateTests
{
	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithNewItem_ShouldInsertIt(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
	{
		await GenericTest.Create(environment)
			.Act(async (repository, context) =>
			{
				ExchangeRate exchangeRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await repository.AddOrUpdate([exchangeRate], CancellationToken.None);
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
	public async Task WithExistingItemDifferentValue_ShouldUpdateValueWithoutDuplicating(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				ExchangeRate updatedRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0900m
				};

				await repository.AddOrUpdate([updatedRate], CancellationToken.None);
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
	public async Task WithExistingItemSameValue_ShouldLeaveItUnchanged(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				ExchangeRate sameRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0800m
				};

				await repository.AddOrUpdate([sameRate], CancellationToken.None);
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(1);
				exchangeRates.First().Value.Should().Be(1.0800m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithMixedBatch_ShouldInsertNewAndUpdateExisting(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				ExchangeRate updatedEurUsdRate = new()
				{
					Date = new DateOnly(2023, 6, 10),
					CurrencyPair = "EURUSD",
					Value = 1.0900m
				};

				ExchangeRate newEurRonRate = new()
				{
					Date = new DateOnly(2023, 6, 11),
					CurrencyPair = "EURRON",
					Value = 4.9700m
				};

				await repository.AddOrUpdate([updatedEurUsdRate, newEurRonRate], CancellationToken.None);
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();

				exchangeRates.Should().HaveCount(2);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURUSD" && x.Value == 1.0900m);
				exchangeRates.Should().ContainSingle(x => x.CurrencyPair == (CurrencyPair)"EURRON" && x.Value == 4.9700m);
			})
			.ExecuteAsync();
	}

	[Theory]
	[TestEnvironments<IExchangeRateRepository, ITestBackDoor>]
	public async Task WithEmptyCollection_ShouldNotChangeDatabase(ITestEnvironment<IExchangeRateRepository, ITestBackDoor> environment)
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
				await repository.AddOrUpdate([], CancellationToken.None);
			})
			.Assert(async (backDoor, context) =>
			{
				List<ExchangeRate> exchangeRates = await backDoor.GetAllExchangeRatesAsync();
				exchangeRates.Should().HaveCount(1);
			})
			.ExecuteAsync();
	}
}
