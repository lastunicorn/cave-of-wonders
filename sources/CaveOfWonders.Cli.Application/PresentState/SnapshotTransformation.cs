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

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

internal class SnapshotTransformation
{
    public List<PotSnapshot> Snapshots { get; set; }

    public string DestinationCurrency { get; set; }

    public List<ConversionRate> ConversionRates { get; set; }

    public IEnumerable<PotInstance> Transform()
    {
        return Snapshots
            .Select(ToPotInstance)
            .ToList();
    }

    private PotInstance ToPotInstance(PotSnapshot potSnapshot)
    {
        PotInstance potInstance = new(potSnapshot);

        string originalCurrency = potInstance.OriginalValue.Currency;
        if (originalCurrency != DestinationCurrency)
        {
            potInstance.ConvertedValue = TryConvert(potInstance.OriginalValue, DestinationCurrency);
        }

        return potInstance;
    }

    private CurrencyValue TryConvert(CurrencyValue originalValue, string destinationCurrency)
    {
        ConversionRate conversionRate = ConversionRates
            .FirstOrDefault(x => x.CanConvert(originalValue.Currency, destinationCurrency));

        if (conversionRate == null)
            return null;

        return new CurrencyValue
        {
            Currency = destinationCurrency,
            Value = originalValue.Currency == conversionRate.SourceCurrency
                ? conversionRate.Convert(originalValue.Value)
                : conversionRate.ConvertBack(originalValue.Value)
        };
    }
}