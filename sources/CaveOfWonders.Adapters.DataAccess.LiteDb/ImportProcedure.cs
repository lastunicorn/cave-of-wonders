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

using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Entities;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb.Utils;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using LiteDB;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.LiteDb;

internal class ImportProcedure
{
    private readonly ILiteCollection<ExchangeRateDbEntity> exchangeRatesCollection;

    private BucketCollection<DateTime, ExchangeRateDbEntity> scheduledItems = new();

    public ImportReport Report { get; private set; }

    public ImportProcedure(ILiteCollection<ExchangeRateDbEntity> exchangeRatesCollection)
    {
        this.exchangeRatesCollection = exchangeRatesCollection ?? throw new ArgumentNullException(nameof(exchangeRatesCollection));
    }

    public void Execute(IEnumerable<ExchangeRate> exchangeRates)
    {
        Report = new ImportReport();
        scheduledItems = new BucketCollection<DateTime, ExchangeRateDbEntity>();

        foreach (ExchangeRate exchangeRate in exchangeRates)
        {
            ProcessNew(exchangeRate);

            if (Report.TotalCount % 1000 == 0)
                Console.WriteLine(Report.TotalCount);
        }

        if (scheduledItems.Count > 0)
            exchangeRatesCollection.Upsert(scheduledItems);
    }

    private void ProcessNew(ExchangeRate exchangeRate)
    {
        ExchangeRateDbEntity existingExchangeRate = RetrieveSimilarExisting(exchangeRate);

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

    private ExchangeRateDbEntity RetrieveSimilarExisting(ExchangeRate exchangeRate)
    {
        string currencyPairAsString = exchangeRate.CurrencyPair;
        return exchangeRatesCollection.FindOne(x => x.Date == exchangeRate.Date && x.CurrencyPair == currencyPairAsString);
    }

    private ExchangeRateDbEntity RetrieveSimilarScheduled(ExchangeRate exchangeRate)
    {
        string currencyPairAsString = exchangeRate.CurrencyPair;

        List<ExchangeRateDbEntity> bucket = scheduledItems.GetBucket(exchangeRate.Date);

        return bucket?
            .FirstOrDefault(x => x.CurrencyPair == currencyPairAsString);
    }

    private void UpdateExistingIfNecessary(ExchangeRateDbEntity existingExchangeRate, ExchangeRate exchangeRate)
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

            List<ExchangeRateDbEntity> bucket = scheduledItems.GetOrCreateBucket(existingExchangeRate.Date);
            bucket.Add(existingExchangeRate);

            Report.ExistingUpdatedCount++;
        }
    }

    private void UpdateNewIfNecessary(ExchangeRateDbEntity existingExchangeRate, ExchangeRate exchangeRate)
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

            List<ExchangeRateDbEntity> bucket = scheduledItems.GetOrCreateBucket(existingExchangeRate.Date);
            bucket.Add(existingExchangeRate);

            Report.NewDuplicateDifferentCount++;
        }
    }

    private void AddNew(ExchangeRate exchangeRate)
    {
        ExchangeRateDbEntity exchangeRateDbEntity = new(exchangeRate);

        List<ExchangeRateDbEntity> bucket = scheduledItems.GetOrCreateBucket(exchangeRateDbEntity.Date);
        bucket.Add(exchangeRateDbEntity);

        Report.AddedCount++;
    }
}