using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ImportExchangeRatesTests;

public class ImportOperationTests
{
	private readonly Mock<IExchangeRateRepository> exchangeRateRepository;
	private readonly ImportOperation importOperation;

	public ImportOperationTests()
	{
		exchangeRateRepository = new Mock<IExchangeRateRepository>();
		importOperation = new ImportOperation(exchangeRateRepository.Object);
	}

	[Fact]
	public async Task HavingNewRate_WhenImported_ThenAddedCountIsIncrementedAndRateIsPassedToAddOrUpdate()
	{
		ExchangeRate newRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync((ExchangeRate)null);

		ExchangeRateImportReport report = await importOperation.Execute([newRate], CancellationToken.None);

		report.AddedCount.Should().Be(1);
		report.TotalCount.Should().Be(1);

		exchangeRateRepository.Verify(x => x.AddOrUpdate(
			It.Is<IEnumerable<ExchangeRate>>(rates => rates.Count() == 1 &&
				rates.Single().CurrencyPair == (CurrencyPair)"EURUSD" &&
				rates.Single().Date == new DateOnly(2023, 6, 10) &&
				rates.Single().Value == 1.0800m),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HavingExistingIdenticalRate_WhenImported_ThenExistingIdenticalCountIsIncrementedAndAddOrUpdateIsNotCalled()
	{
		ExchangeRate incomingRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		ExchangeRate existingRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync(existingRate);

		ExchangeRateImportReport report = await importOperation.Execute([incomingRate], CancellationToken.None);

		report.ExistingIdenticalCount.Should().Be(1);
		report.TotalCount.Should().Be(1);
		report.Updates.Should().BeEmpty();

		exchangeRateRepository.Verify(x => x.AddOrUpdate(It.IsAny<IEnumerable<ExchangeRate>>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task HavingExistingRateWithDifferentValue_WhenImported_ThenExistingUpdatedCountIsIncrementedAndUpdateReportIsRecorded()
	{
		ExchangeRate incomingRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0900m
		};

		ExchangeRate existingRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync(existingRate);

		ExchangeRateImportReport report = await importOperation.Execute([incomingRate], CancellationToken.None);

		report.ExistingUpdatedCount.Should().Be(1);
		report.TotalCount.Should().Be(1);
		report.Updates.Should().ContainSingle(x =>
			x.Date == new DateOnly(2023, 6, 10) &&
			x.CurrencyPair == (CurrencyPair)"EURUSD" &&
			x.OldValue == 1.0800m &&
			x.NewValue == 1.0900m);

		exchangeRateRepository.Verify(x => x.AddOrUpdate(
			It.Is<IEnumerable<ExchangeRate>>(rates => rates.Count() == 1 && rates.Single().Value == 1.0900m),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HavingTwoIdenticalRatesInTheSameBatch_WhenImported_ThenNewDuplicateIdenticalCountIsIncrementedAndOnlyOneItemIsScheduled()
	{
		ExchangeRate rate1 = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		ExchangeRate rate2 = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync((ExchangeRate)null);

		ExchangeRateImportReport report = await importOperation.Execute([rate1, rate2], CancellationToken.None);

		report.AddedCount.Should().Be(1);
		report.NewDuplicateIdenticalCount.Should().Be(1);
		report.TotalCount.Should().Be(2);
		report.Duplicates.Should().BeEmpty();

		exchangeRateRepository.Verify(x => x.AddOrUpdate(
			It.Is<IEnumerable<ExchangeRate>>(rates => rates.Count() == 1),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HavingTwoDifferentRatesForSamePairAndDateInTheSameBatch_WhenImported_ThenNewDuplicateDifferentCountIsIncrementedAndScheduledItemCarriesLastValue()
	{
		ExchangeRate rate1 = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		ExchangeRate rate2 = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0850m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync((ExchangeRate)null);

		ExchangeRateImportReport report = await importOperation.Execute([rate1, rate2], CancellationToken.None);

		report.AddedCount.Should().Be(1);
		report.NewDuplicateDifferentCount.Should().Be(1);
		report.TotalCount.Should().Be(2);
		report.Duplicates.Should().ContainSingle(x =>
			x.Date == new DateOnly(2023, 6, 10) &&
			x.CurrencyPair == (CurrencyPair)"EURUSD" &&
			x.Value1 == 1.0800m &&
			x.Value2 == 1.0850m);

		exchangeRateRepository.Verify(x => x.AddOrUpdate(
			It.Is<IEnumerable<ExchangeRate>>(rates => rates.Count() == 1 && rates.Single().Value == 1.0850m),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task HavingMixedInput_WhenImported_ThenAllCountersAddUpToTotalCount()
	{
		ExchangeRate newRate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		ExchangeRate existingIdenticalRate = new()
		{
			Date = new DateOnly(2023, 6, 11),
			CurrencyPair = "EURUSD",
			Value = 4.9700m
		};

		ExchangeRate existingUpdatedRate = new()
		{
			Date = new DateOnly(2023, 6, 12),
			CurrencyPair = "EURUSD",
			Value = 4.6500m
		};

		exchangeRateRepository
			.Setup(x => x.Get((CurrencyPair)"EURUSD", new DateOnly(2023, 6, 10)))
			.ReturnsAsync((ExchangeRate)null);

		exchangeRateRepository
			.Setup(x => x.Get((CurrencyPair)"EURUSD", new DateOnly(2023, 6, 11)))
			.ReturnsAsync(new ExchangeRate { Date = new DateOnly(2023, 6, 11), CurrencyPair = "EURUSD", Value = 4.9700m });

		exchangeRateRepository
			.Setup(x => x.Get((CurrencyPair)"EURUSD", new DateOnly(2023, 6, 12)))
			.ReturnsAsync(new ExchangeRate { Date = new DateOnly(2023, 6, 12), CurrencyPair = "EURUSD", Value = 4.6000m });

		ExchangeRateImportReport report = await importOperation.Execute([newRate, existingIdenticalRate, existingUpdatedRate], CancellationToken.None);

		report.AddedCount.Should().Be(1);
		report.ExistingIdenticalCount.Should().Be(1);
		report.ExistingUpdatedCount.Should().Be(1);
		report.NewDuplicateIdenticalCount.Should().Be(0);
		report.NewDuplicateDifferentCount.Should().Be(0);
		report.TotalCount.Should().Be(3);
	}

	[Fact]
	public async Task HavingCancellationRequested_WhenImported_ThenThrowsAndAddOrUpdateIsNotCalled()
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
			Value = 1.0900m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync((ExchangeRate)null);

		using CancellationTokenSource cancellationTokenSource = new();
		cancellationTokenSource.Cancel();

		Func<Task> action = async () => await importOperation.Execute([rate1, rate2], cancellationTokenSource.Token);

		await action.Should().ThrowAsync<OperationCanceledException>();

		exchangeRateRepository.Verify(x => x.AddOrUpdate(It.IsAny<IEnumerable<ExchangeRate>>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task HavingInputEnumerable_WhenImported_ThenItIsEnumeratedExactlyOnce()
	{
		ExchangeRate rate = new()
		{
			Date = new DateOnly(2023, 6, 10),
			CurrencyPair = "EURUSD",
			Value = 1.0800m
		};

		exchangeRateRepository
			.Setup(x => x.Get(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>()))
			.ReturnsAsync((ExchangeRate)null);

		int enumerationCount = 0;

		IEnumerable<ExchangeRate> CountingSource()
		{
			enumerationCount++;
			yield return rate;
		}

		await importOperation.Execute(CountingSource(), CancellationToken.None);

		enumerationCount.Should().Be(1);
	}
}
