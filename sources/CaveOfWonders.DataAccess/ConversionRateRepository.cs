// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.DataAccess;

public class ConversionRateRepository : IConversionRateRepository
{
    public Task<IEnumerable<ConversionRate>> GetAll(DateTime date)
    {
        IEnumerable<ConversionRate> conversionRates = new List<ConversionRate>
        {
            new()
            {
                SourceCurrency = "EUR",
                DestinationCurrency = "RON",
                Date = new DateTime(2023, 05, 12),
                Value = 4.9294f
            },
            new()
            {
                SourceCurrency = "EUR",
                DestinationCurrency = "RON",
                Date = new DateTime(2023, 04, 13),
                Value = 4.9432f
            },
            new()
            {
                SourceCurrency = "XAU",
                DestinationCurrency = "RON",
                Date = new DateTime(2023, 05, 12),
                Value = 291.1368f
            },
            new()
            {
                SourceCurrency = "XAU",
                DestinationCurrency = "RON",
                Date = new DateTime(2023, 04, 13),
                Value = 292.1414f
            }
        };

        return Task.FromResult(conversionRates.Where(x => x.Date == date));
    }
}