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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess.NbrFiles.NbrModels;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.BnrAccess;

internal static class NbrRowExtensions
{
    public static IEnumerable<BnrExchangeRate> ToExchangeRates(this NbrCube cube, string origCurrency)
    {
        return cube.Rates
            .Where(x => x.Value != null && x.Value != "-")
            .Select(x => x.ToExchangeRate(cube, origCurrency));
    }

    private static BnrExchangeRate ToExchangeRate(this NbrRate nbrRate, NbrCube cube, string origCurrency)
    {
        int multiplier = 1;

        if (nbrRate.Multiplier != null)
            multiplier = int.Parse(nbrRate.Multiplier);

        return new BnrExchangeRate
        {
            Date = DateTime.Parse(cube.Date, CultureInfo.InvariantCulture),
            CurrencyPair = (nbrRate.Currency + origCurrency).ToUpper(),
            Value = decimal.Parse(nbrRate.Value, CultureInfo.InvariantCulture) / multiplier
        };
    }
}