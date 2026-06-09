using DustInTheWind.Bnr.Toolkit.ExchangeRates;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

public class BnrService : IBnrService
{
    public Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFrom(string filePath, CancellationToken cancellationToken)
    {
        using FileStream fileStream = File.OpenRead(filePath);
        BnrDocument bnrDocument = BnrDocument.Load(fileStream);

        IEnumerable<BnrExchangeRate> exchangeRates = bnrDocument.DataSet.Table.Rows
            .SelectMany(x => x.ToExchangeRates());

        return Task.FromResult(exchangeRates);
    }

    public async Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbr(string filePath, CancellationToken cancellationToken)
    {
        await using FileStream fileStream = File.OpenRead(filePath);
        ExchangeRatesDocument document = await ExchangeRatesDocument.LoadAsync(fileStream);

        return document.Cubes
            .SelectMany(x => x.ToExchangeRates(document.ReferenceCurrency));
    }

    public async Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbrOnline(int year, CancellationToken cancellationToken)
    {
        ExchangeRatesDocument document = await ExchangeRatesOnlineDocument.LoadForYear(year, cancellationToken);

        return document.Cubes
            .SelectMany(x => x.ToExchangeRates(document.ReferenceCurrency));
    }
}