// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;

public class ExchangeRatesNotFoundNote : INote
{
    public List<CurrencyPair> CurrencyPairs { get; set; }
    
    public DateTime Date { get; set; }

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