namespace DustInTheWind.CaveOfWonders.Ports.BnrAccess;

public interface IBnr
{
    Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFrom(string filePath, CancellationToken cancellationToken);

    Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbr(string filePath, CancellationToken cancellationToken);

    Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbrOnline(int year, CancellationToken cancellationToken);
}