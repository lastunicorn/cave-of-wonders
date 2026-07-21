using DustInTheWind.Bnr.Toolkit.ExchangeRates;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

public class BnrService : IBnrService
{
	public async Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromFile(string filePath, CancellationToken cancellationToken)
	{
		await using FileStream fileStream = File.OpenRead(filePath);
		ExchangeRatesDocument document = await ExchangeRatesDocument.LoadAsync(fileStream);

		return document.DailyExchangeRates
			.SelectMany(x => x.ToExchangeRates(document.ReferenceCurrency));
	}

	public async Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromOnline(int year, CancellationToken cancellationToken)
	{
		ExchangeRatesDocument document = await ExchangeRatesOnlineDocument.LoadByYear(year, cancellationToken);

		return document.DailyExchangeRates
			.SelectMany(x => x.ToExchangeRates(document.ReferenceCurrency));
	}
}