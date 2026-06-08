using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.BnrFiles;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

public class Bnr : IBnr
{
    public Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFrom(string filePath, CancellationToken cancellationToken)
    {
        using FileStream fileStream = File.OpenRead(filePath);
        BnrDocument bnrDocument = BnrDocument.Load(fileStream);

        IEnumerable<BnrExchangeRate> exchangeRates = bnrDocument.DataSet.Table.Rows
            .SelectMany(x => x.ToExchangeRates());

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbr(string filePath, CancellationToken cancellationToken)
    {
        using FileStream fileStream = File.OpenRead(filePath);

        IEnumerable<BnrExchangeRate> exchangeRates = GetExchangeRatesFromNbrStream(fileStream);
        return Task.FromResult(exchangeRates);
    }

    public async Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbrOnline(int year, CancellationToken cancellationToken)
    {
        await using NbrOnlineStream stream = new(year);
        await stream.Open(cancellationToken);

        return GetExchangeRatesFromNbrStream(stream);
    }

    private static IEnumerable<BnrExchangeRate> GetExchangeRatesFromNbrStream(Stream stream)
    {
        NbrDocument nbrDocument = NbrDocument.Load(stream);

        IEnumerable<BnrExchangeRate> exchangeRates = nbrDocument.DataSet.Body.Cubes
            .SelectMany(x => x.ToExchangeRates(nbrDocument.DataSet.Body.OrigCurrency));

        return exchangeRates;
    }
}