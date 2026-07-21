namespace DustInTheWind.CaveOfWonders.Ports.BnrAccess;

public interface IBnrService
{
	Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromFile(string filePath, CancellationToken cancellationToken);

	Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromOnline(int year, CancellationToken cancellationToken);
}