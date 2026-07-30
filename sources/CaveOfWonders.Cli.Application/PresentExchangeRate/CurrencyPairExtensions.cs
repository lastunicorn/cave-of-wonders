using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

internal static class CurrencyPairExtensions
{
	public static IEnumerable<CurrencyPair> ParseCurrencyPairs(this string currencyPair)
	{
		if (currencyPair.IsNullOrEmpty())
		{
			yield return CurrencyPair.Empty;
		}
		else if (currencyPair.Contains(',') || currencyPair.Contains(';'))
		{
			string[] pairs = currencyPair.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries);

			foreach (string pair in pairs)
				yield return new CurrencyPair(pair.Trim());
		}
		else
		{
			yield return new CurrencyPair(currencyPair.Trim());
		}
	}
}