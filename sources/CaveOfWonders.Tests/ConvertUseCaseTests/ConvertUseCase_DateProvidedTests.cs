using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Infrastructure;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using FluentAssertions;
using Moq;

namespace CaveOfWonders.Tests.ConvertUseCaseTests;

public class ConvertUseCase_DateProvidedTests
{
    private readonly ConvertUseCase convertUseCase;
    private readonly Mock<IExchangeRateRepository> exchangeRateRepository;

    public ConvertUseCase_DateProvidedTests()
    {
        Mock<IUnitOfWork> unitOfWork = new();
        exchangeRateRepository = new Mock<IExchangeRateRepository>();

        convertUseCase = new(unitOfWork.Object, Mock.Of<ISystemClock>());

        unitOfWork
            .SetupGet(x => x.ExchangeRateRepository)
            .Returns(exchangeRateRepository.Object);
    }

    [Fact]
    public async Task HavingDateProvided_ThenExchangeRateIsRequestedForThatDate()
    {
        ConvertRequest convertRequest = new()
        {
            InitialValue = 100,
            CurrencyPair = new CurrencyPair("EURRON"),
            Date = new DateTime(2000, 06, 04)
        };

        try
        {
            _ = await convertUseCase.Handle(convertRequest, CancellationToken.None);
        }
        catch { }

        CurrencyPair expectedCurrencyPair = new("EURRON");
        DateTime expectedDate = new(2000, 06, 04);

        exchangeRateRepository.Verify(x => x.GetForLatestDayAvailable(expectedCurrencyPair, expectedDate, true), Times.Once);
    }

    [Fact]
    public async Task HavingDateProvided_AndExchangeRateDoesNotExistInStorage_ThenThrows()
    {
        exchangeRateRepository
            .Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(null as ExchangeRate));

        ConvertRequest convertRequest = new()
        {
            InitialValue = 100,
            CurrencyPair = new CurrencyPair("EURRON"),
            Date = new DateTime(2000, 06, 04)
        };

        Func<Task> action = async () => await convertUseCase.Handle(convertRequest, CancellationToken.None);

        await action.Should().ThrowAsync<ExchangeRateNotFoundException>();
    }

    [Fact]
    public async Task HavingDateProvided_AndExchangeRateExistsInStorage_ThenReturnsConvertedValue()
    {
        exchangeRateRepository
            .Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(new ExchangeRate()
            {
                Value = 2,
                CurrencyPair = new CurrencyPair("EURRON"),
                Date = new DateTime(2000, 06, 04)
            }));

        ConvertRequest convertRequest = new()
        {
            InitialValue = 102,
            CurrencyPair = new CurrencyPair("EURRON"),
            Date = new DateTime(2000, 06, 04)
        };

        ConvertResponse response = await convertUseCase.Handle(convertRequest, CancellationToken.None);

        response.InitialValue.Should().Be(102);
        response.ConvertedValue.Should().Be(204);
        response.IsDateCurrent.Should().BeTrue();
        response.ExchangeRate.Should().NotBeNull();
        response.ExchangeRate.SourceCurrency.Should().Be("EUR");
        response.ExchangeRate.DestinationCurrency.Should().Be("RON");
        response.ExchangeRate.Value.Should().Be(2);
        response.ExchangeRate.Date.Should().Be(new DateTime(2000, 06, 04));
    }

    [Fact]
    public async Task HavingDateProvided_AndExchangeRateExistsInStorageForAPreviousDate_ThenReturnsConvertedValue()
    {
        exchangeRateRepository
            .Setup(x => x.GetForLatestDayAvailable(It.IsAny<CurrencyPair>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(new ExchangeRate()
            {
                Value = 2,
                CurrencyPair = new CurrencyPair("EURRON"),
                Date = new DateTime(2000, 06, 01)
            }));

        ConvertRequest convertRequest = new()
        {
            InitialValue = 102,
            CurrencyPair = new CurrencyPair("EURRON"),
            Date = new DateTime(2000, 06, 04)
        };

        ConvertResponse response = await convertUseCase.Handle(convertRequest, CancellationToken.None);

        response.InitialValue.Should().Be(102);
        response.ConvertedValue.Should().Be(204);
        response.IsDateCurrent.Should().BeFalse();
        response.ExchangeRate.Should().NotBeNull();
        response.ExchangeRate.SourceCurrency.Should().Be("EUR");
        response.ExchangeRate.DestinationCurrency.Should().Be("RON");
        response.ExchangeRate.Value.Should().Be(2);
        response.ExchangeRate.Date.Should().Be(new DateTime(2000, 06, 01));
    }
}