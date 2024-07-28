// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CurrencyExchange.Adapters.BnrAccess.BnrFiles;
using DustInTheWind.CurrencyExchange.Adapters.BnrAccess.NbrFiles;
using DustInTheWind.CurrencyExchange.Ports.BnrAccess;

namespace DustInTheWind.CurrencyExchange.Adapters.BnrAccess;

public class Bnr : IBnr
{
    public Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFrom(string filePath)
    {
        using FileStream fileStream = File.OpenRead(filePath);
        BnrDocument bnrDocument = BnrDocument.Load(fileStream);

        IEnumerable<BnrExchangeRate> exchangeRates = bnrDocument.DataSet.Table.Rows
            .SelectMany(x => x.ToExchangeRates());

        return Task.FromResult(exchangeRates);
    }

    public Task<IEnumerable<BnrExchangeRate>> GetExchangeRatesFromNbr(string filePath)
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