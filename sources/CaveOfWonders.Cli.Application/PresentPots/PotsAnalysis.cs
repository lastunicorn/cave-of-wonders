// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

internal class PotsAnalysis
{
    private readonly CurrenciesConvertor currenciesConvertor;

    public List<PotInstanceInfo> PotInstanceInfos { get; set; }

    public string TargetCurrency { get; set; }

    public DateTime TargetDate { get; set; }

    public decimal TotalValue { get; private set; }

    public List<CurrencyTotalOverview> currencyTotalOverviews;

    public PotsAnalysis(CurrenciesConvertor currenciesConvertor)
    {
        this.currenciesConvertor = currenciesConvertor ?? throw new ArgumentNullException(nameof(currenciesConvertor));
    }

    public async Task Calculate()
    {
        TotalValue = PotInstanceInfos.Sum(x => x.NormalizedValue?.Value ?? 0);
        currencyTotalOverviews = await CalculateCurrencyTotalOverviews();
    }

    private async Task<List<CurrencyTotalOverview>> CalculateCurrencyTotalOverviews()
    {
        // Group pot instances by currency and calculate the sum for each currency
        IEnumerable<IGrouping<string, PotInstanceInfo>> currencyGroups = PotInstanceInfos
            .Where(x => x.OriginalValue != null)
            .GroupBy(x => x.OriginalValue.Currency);

        List<CurrencyTotalOverview> overviews = [];

        foreach (IGrouping<string, PotInstanceInfo> group in currencyGroups)
        {
            string currency = group.Key;
            decimal value = group.Sum(x => x.OriginalValue?.Value ?? 0);

            CurrencyValue currencyValue = new()
            {
                Currency = currency,
                Value = value,
                Date = TargetDate
            };

            CurrencyValue normalizedValue = await CalculateNormalizedValue(currencyValue);

            // Calculate percentage
            decimal percentage = TotalValue > 0
                ? (normalizedValue.Value / TotalValue) * 100
                : 0;

            // Create and add the overview
            CurrencyTotalOverview overview = new()
            {
                Value = currencyValue,
                NormalizedValue = normalizedValue,
                Percentage = percentage
            };

            overviews.Add(overview);
        }

        return overviews;
    }

    private async Task<CurrencyValue> CalculateNormalizedValue(CurrencyValue currencyValue)
    {
        return currencyValue.Currency == TargetCurrency
            ? currencyValue
            : await currenciesConvertor.Convert(currencyValue, TargetCurrency, TargetDate);
    }
}