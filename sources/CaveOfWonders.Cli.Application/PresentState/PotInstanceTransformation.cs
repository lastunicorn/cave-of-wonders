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

using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

internal class PotInstanceTransformation
{
    public DateTime Date { get; set; }

    public List<PotInstance> Instances { get; set; }

    public string DestinationCurrency { get; set; }

    public List<ExchangeRate> ExchangeRates { get; set; }

    public IEnumerable<PotInstanceInfo> Transform()
    {
        return Instances
            .Select(ToPotInstance)
            .ToList();
    }

    private PotInstanceInfo ToPotInstance(PotInstance potInstance)
    {
        PotInstanceInfo potInstanceInfo = new()
        {
            Id = potInstance.Pot.Id,
            Name = potInstance.Pot.Name,
            IsActive = potInstance.Pot.IsActive(Date)
        };

        if (potInstance.Gem != null)
        {
            potInstanceInfo.OriginalValue = new CurrencyValue
            {
                Currency = potInstance.Pot.Currency,
                Value = potInstance.Gem.Value
            };

            potInstanceInfo.Date = potInstance.Gem.Date;
        }

        if (potInstanceInfo.OriginalValue != null)
        {
            string originalCurrency = potInstanceInfo.OriginalValue.Currency;

            if (originalCurrency != DestinationCurrency)
                potInstanceInfo.NormalizedValue = TryConvert(potInstanceInfo.OriginalValue, DestinationCurrency);
        }

        return potInstanceInfo;
    }

    private CurrencyValue TryConvert(CurrencyValue originalValue, string destinationCurrency)
    {
        ExchangeRate exchangeRate = ExchangeRates
            .FirstOrDefault(x => x.CanConvert(originalValue.Currency, destinationCurrency));

        if (exchangeRate == null)
            return null;

        return new CurrencyValue
        {
            Currency = destinationCurrency,
            Value = originalValue.Currency == exchangeRate.CurrencyPair.Currency1
                ? exchangeRate.Convert(originalValue.Value)
                : exchangeRate.ConvertBack(originalValue.Value)
        };
    }
}