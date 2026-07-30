using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

[Serializable]
public class ExchangeRateNotFoundException : Exception
{
	private const string DefaultMessage = "Exchange rate for {0} and date {1:d} was not found.";

	public ExchangeRateNotFoundException(CurrencyPair currencyPair, DateOnly date)
		: base(string.Format(DefaultMessage, currencyPair, date))
	{
	}

	public ExchangeRateNotFoundException(IEnumerable<CurrencyPair> currencyPairs, DateOnly date)
		: base(string.Format(DefaultMessage, string.Join(", ", currencyPairs), date))
	{
	}

	public ExchangeRateNotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}