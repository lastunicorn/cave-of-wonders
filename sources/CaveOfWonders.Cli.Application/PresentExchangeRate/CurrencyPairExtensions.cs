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