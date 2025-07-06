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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Utils;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;

internal class ImportProcedure
{
    private readonly List<ExchangeRate> exchangeRatesFromDatabase;

    private BucketCollection<DateTime, ExchangeRate> scheduledItems = new();

    public ExchangeRateImportReport Report { get; private set; }

    public ImportProcedure(List<ExchangeRate> exchangeRatesFromDatabase)
    {
        this.exchangeRatesFromDatabase = exchangeRatesFromDatabase ?? throw new ArgumentNullException(nameof(exchangeRatesFromDatabase));
    }

    public void Execute(IEnumerable<ExchangeRate> exchangeRates)
    {
        Report = new ExchangeRateImportReport();
        scheduledItems = new BucketCollection<DateTime, ExchangeRate>();

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            ProcessNew(exchangeRate);

            if (Report.TotalCount % 1000 == 0)
                Console.WriteLine(Report.TotalCount);
        }

        if (scheduledItems.Count > 0)
        {
            foreach (ExchangeRate exchangeRate in exchangeRates)
                UpdateOrAddInDatabase(exchangeRate);
        }
    }

    private void UpdateOrAddInDatabase(ExchangeRate exchangeRate)
    {
        ExchangeRate existingExchangeRate = exchangeRatesFromDatabase
            .FirstOrDefault(x => x.CurrencyPair == exchangeRate.CurrencyPair && x.Date == exchangeRate.Date);

        if (existingExchangeRate != null)
        {
            existingExchangeRate.Value = exchangeRate.Value;
        }
        else
        {
            ExchangeRate newExchangeRate = new()
            {
                CurrencyPair = exchangeRate.CurrencyPair,
                Date = exchangeRate.Date,
                Value = exchangeRate.Value
            };

            exchangeRatesFromDatabase.Add(newExchangeRate);
        }
    }

    private void ProcessNew(ExchangeRate exchangeRate)
    {
        ExchangeRate existingExchangeRate = RetrieveSimilarExisting(exchangeRate);

        if (existingExchangeRate != null)
        {
            UpdateExistingIfNecessary(existingExchangeRate, exchangeRate);
        }
        else
        {
            existingExchangeRate = RetrieveSimilarScheduled(exchangeRate);

            if (existingExchangeRate != null)
                UpdateNewIfNecessary(existingExchangeRate, exchangeRate);
            else
                AddNew(exchangeRate);
        }

        Report.TotalCount++;
    }

    private ExchangeRate RetrieveSimilarExisting(ExchangeRate exchangeRate)
    {
        string currencyPairAsString = exchangeRate.CurrencyPair;
        return exchangeRatesFromDatabase.FirstOrDefault(x => x.Date == exchangeRate.Date && x.CurrencyPair == currencyPairAsString);
    }

    private ExchangeRate RetrieveSimilarScheduled(ExchangeRate exchangeRate)
    {
        string currencyPairAsString = exchangeRate.CurrencyPair;

        List<ExchangeRate> bucket = scheduledItems.GetBucket(exchangeRate.Date);

        return bucket?
            .FirstOrDefault(x => x.CurrencyPair == currencyPairAsString);
    }

    private void UpdateExistingIfNecessary(ExchangeRate existingExchangeRate, ExchangeRate exchangeRate)
    {
        bool areEqualValues = existingExchangeRate.Value == exchangeRate.Value;

        if (areEqualValues)
        {
            Report.ExistingIdenticalCount++;
        }
        else
        {
            UpdateReport updateReport = new()
            {
                Date = existingExchangeRate.Date,
                CurrencyPair = existingExchangeRate.CurrencyPair,
                OldValue = existingExchangeRate.Value,
                NewValue = exchangeRate.Value
            };
            Report.Updates.Add(updateReport);

            existingExchangeRate.Value = exchangeRate.Value;

            List<ExchangeRate> bucket = scheduledItems.GetOrCreateBucket(existingExchangeRate.Date);
            bucket.Add(existingExchangeRate);

            Report.ExistingUpdatedCount++;
        }
    }

    private void UpdateNewIfNecessary(ExchangeRate existingExchangeRate, ExchangeRate exchangeRate)
    {
        bool areEqualValues = existingExchangeRate.Value == exchangeRate.Value;

        if (areEqualValues)
        {
            Report.NewDuplicateIdenticalCount++;
        }
        else
        {
            DuplicateReport duplicateReport = new()
            {
                Date = existingExchangeRate.Date,
                CurrencyPair = existingExchangeRate.CurrencyPair,
                Value1 = existingExchangeRate.Value,
                Value2 = exchangeRate.Value
            };
            Report.Duplicates.Add(duplicateReport);

            existingExchangeRate.Value = exchangeRate.Value;

            List<ExchangeRate> bucket = scheduledItems.GetOrCreateBucket(existingExchangeRate.Date);
            bucket.Add(existingExchangeRate);

            Report.NewDuplicateDifferentCount++;
        }
    }

    private void AddNew(ExchangeRate exchangeRate)
    {
        List<ExchangeRate> bucket = scheduledItems.GetOrCreateBucket(exchangeRate.Date);
        bucket.Add(exchangeRate);

        Report.AddedCount++;
    }
}