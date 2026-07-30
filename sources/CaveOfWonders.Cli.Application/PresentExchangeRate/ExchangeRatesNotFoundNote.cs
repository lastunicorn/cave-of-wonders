using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class ExchangeRatesNotFoundNote : INote
{
	public List<CurrencyPair> CurrencyPairs { get; set; }

	public DateOnly Date { get; set; }

	public override string ToString()
	{
		if (CurrencyPairs == null || CurrencyPairs.Count == 0)
		{
			return $"Exchange rates were not found for the date {Date:d}. The last available value was returned.";
		}
		else if (CurrencyPairs.Count == 1)
		{
			return $"Exchange rate for {CurrencyPairs[0]} was not found for the date {Date:d}. The last available value was returned.";
		}
		else
		{
			string currencyPairsString = string.Join(", ", CurrencyPairs.Select(x => x.ToString()));
			return $"Exchange rates for {currencyPairsString} were not found for the date {Date:d}. The last available values were returned.";
		}
	}
}