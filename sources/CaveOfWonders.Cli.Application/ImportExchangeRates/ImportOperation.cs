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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

internal class ImportOperation
{
    private readonly IExchangeRateRepository exchangeRateRepository;

    public ImportOperation(IExchangeRateRepository exchangeRateRepository)
    {
        this.exchangeRateRepository = exchangeRateRepository ?? throw new ArgumentNullException(nameof(exchangeRateRepository));
    }

    public async Task<ExchangeRateImportReport> Execute(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        ExchangeRateImportReport report = new();
        Dictionary<(DateOnly, string), ExchangeRate> scheduledItems = new();

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string pairAsString = exchangeRate.CurrencyPair;
            (DateOnly, string) key = (exchangeRate.Date, pairAsString);

            ExchangeRate existing = await exchangeRateRepository.Get(exchangeRate.CurrencyPair, exchangeRate.Date);

            if (existing != null)
            {
                ProcessExisting(existing, exchangeRate, report, scheduledItems, key);
            }
            else if (scheduledItems.TryGetValue(key, out ExchangeRate scheduled))
            {
                ProcessDuplicate(scheduled, exchangeRate, report);
            }
            else
            {
                scheduledItems[key] = new ExchangeRate
                {
                    CurrencyPair = exchangeRate.CurrencyPair,
                    Date = exchangeRate.Date,
                    Value = exchangeRate.Value
                };
                report.AddedCount++;
            }

            report.TotalCount++;
        }

        if (scheduledItems.Count > 0)
            await exchangeRateRepository.AddOrUpdate(scheduledItems.Values, cancellationToken);

        return report;
    }

    private static void ProcessExisting(ExchangeRate existing, ExchangeRate exchangeRate, ExchangeRateImportReport report,
        Dictionary<(DateOnly, string), ExchangeRate> scheduledItems, (DateOnly, string) key)
    {
        if (existing.Value == exchangeRate.Value)
        {
            report.ExistingIdenticalCount++;
        }
        else
        {
            report.Updates.Add(new UpdateReport
            {
                Date = existing.Date,
                CurrencyPair = existing.CurrencyPair,
                OldValue = existing.Value,
                NewValue = exchangeRate.Value
            });

            scheduledItems[key] = new ExchangeRate
            {
                CurrencyPair = existing.CurrencyPair,
                Date = existing.Date,
                Value = exchangeRate.Value
            };

            report.ExistingUpdatedCount++;
        }
    }

    private static void ProcessDuplicate(ExchangeRate scheduled, ExchangeRate exchangeRate, ExchangeRateImportReport report)
    {
        if (scheduled.Value == exchangeRate.Value)
        {
            report.NewDuplicateIdenticalCount++;
        }
        else
        {
            report.Duplicates.Add(new DuplicateReport
            {
                Date = scheduled.Date,
                CurrencyPair = scheduled.CurrencyPair,
                Value1 = scheduled.Value,
                Value2 = exchangeRate.Value
            });

            scheduled.Value = exchangeRate.Value;

            report.NewDuplicateDifferentCount++;
        }
    }
}
