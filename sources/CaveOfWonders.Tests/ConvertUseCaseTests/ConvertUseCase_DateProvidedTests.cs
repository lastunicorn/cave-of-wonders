using DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ConvertUseCaseTests;

public class ConvertUseCase_DateProvidedTests
{
	private readonly ConvertCurrencyUseCase convertCurrencyUseCase;
	private readonly Mock<IExchangeRateRepository> exchangeRateRepository;

	public ConvertUseCase_DateProvidedTests()
	{
		Mock<IUnitOfWork> unitOfWork = new();
		exchangeRateRepository = new Mock<IExchangeRateRepository>();

		convertCurrencyUseCase = new(unitOfWork.Object, Mock.Of<ISystemClock>());

		unitOfWork
			.SetupGet(x => x.ExchangeRateRepository)
			.Returns(exchangeRateRepository.Object);
	}

	[Fact]
	public async Task HavingDateProvided_ThenExchangeRateIsRequestedForThatDate()
	{
		ConvertCurrencyRequest convertCurrencyRequest = new()
		{
			InitialValue = 100,
			CurrencyPair = new CurrencyPair("EURRON"),
			Date = new DateOnly(2000, 06, 04)
		};

		try
		{
			_ = await convertCurrencyUseCase.Handle(convertCurrencyRequest, CancellationToken.None);
		}
		catch { }

		CurrencyPair expectedCurrencyPair = new("EURRON");
		DateOnly expectedDate = new(2000, 06, 04);

		exchangeRateRepository.Verify(x => x.GetForLatestDayAvailable(expectedCurrencyPair, expectedDate, true), Times.Once);
	}

	[Fact]
	public async Task HavingDateProvided_AndExchangeRateDoesNotExistInStorage_ThenThrows()
	{
		exchangeRateRepository
			.Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>(), It.IsAny<bool>()))
			.Returns(Task.FromResult(null as ExchangeRate));

		ConvertCurrencyRequest convertCurrencyRequest = new()
		{
			InitialValue = 100,
			CurrencyPair = new CurrencyPair("EURRON"),
			Date = new DateOnly(2000, 06, 04)
		};

		Func<Task> action = async () => await convertCurrencyUseCase.Handle(convertCurrencyRequest, CancellationToken.None);

		await action.Should().ThrowAsync<ExchangeRateNotFoundException>();
	}

	[Fact]
	public async Task HavingDateProvided_AndExchangeRateExistsInStorage_ThenReturnsConvertedValue()
	{
		exchangeRateRepository
			.Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>(), It.IsAny<bool>()))
			.Returns(Task.FromResult(new ExchangeRate()
			{
				Value = 2,
				CurrencyPair = new CurrencyPair("EURRON"),
				Date = new DateOnly(2000, 06, 04)
			}));

		ConvertCurrencyRequest convertCurrencyRequest = new()
		{
			InitialValue = 102,
			CurrencyPair = new CurrencyPair("EURRON"),
			Date = new DateOnly(2000, 06, 04)
		};

		ConvertCurrencyResponse response = await convertCurrencyUseCase.Handle(convertCurrencyRequest, CancellationToken.None);

		response.InitialValue.Should().Be(102);
		response.ConvertedValue.Should().Be(204);
		response.IsDateCurrent.Should().BeTrue();
		response.ExchangeRate.Should().NotBeNull();
		response.ExchangeRate.SourceCurrency.Should().Be("EUR");
		response.ExchangeRate.DestinationCurrency.Should().Be("RON");
		response.ExchangeRate.Value.Should().Be(2);
		response.ExchangeRate.Date.Should().Be(new DateOnly(2000, 06, 04));
	}

	[Fact]
	public async Task HavingDateProvided_AndExchangeRateExistsInStorageForAPreviousDate_ThenReturnsConvertedValue()
	{
		exchangeRateRepository
			.Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateOnly>(), It.IsAny<bool>()))
			.Returns(Task.FromResult(new ExchangeRate()
			{
				Value = 2,
				CurrencyPair = new CurrencyPair("EURRON"),
				Date = new DateOnly(2000, 06, 01)
			}));

		ConvertCurrencyRequest convertCurrencyRequest = new()
		{
			InitialValue = 102,
			CurrencyPair = new CurrencyPair("EURRON"),
			Date = new DateOnly(2000, 06, 04)
		};

		ConvertCurrencyResponse response = await convertCurrencyUseCase.Handle(convertCurrencyRequest, CancellationToken.None);

		response.InitialValue.Should().Be(102);
		response.ConvertedValue.Should().Be(204);
		response.IsDateCurrent.Should().BeFalse();
		response.ExchangeRate.Should().NotBeNull();
		response.ExchangeRate.SourceCurrency.Should().Be("EUR");
		response.ExchangeRate.DestinationCurrency.Should().Be("RON");
		response.ExchangeRate.Value.Should().Be(2);
		response.ExchangeRate.Date.Should().Be(new DateOnly(2000, 06, 01));
	}
}