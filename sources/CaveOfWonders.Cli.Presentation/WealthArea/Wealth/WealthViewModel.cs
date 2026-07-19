using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Wealth;

internal class WealthViewModel
{
    public CultureInfo Culture { get; set; }

    public DateOnly Date { get; }

    public List<PotSnapshotViewModel> Values { get; }

    public List<ExchangeRateViewModel> ConversionRates { get; }

    public CurrencyValue Total { get; }

    public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; }

    public WealthViewModel(PresentPotsResponse presentPotsResponse)
    {
        Date = presentPotsResponse.Date;

        Values = presentPotsResponse.PotInstances
            .Select(x => new PotSnapshotViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OriginalValue = x.IsActive
                    ? x.Value
                    : null,
                IsValueActual = x.Value?.Date == presentPotsResponse.Date,
                IsValueAlreadyNormal = x.Value?.Currency == x.NormalizedValue?.Currency,
                IsNormalizedCurrent = x.NormalizedValue?.Date == Date,
                Date = x.IsActive
                    ? x.Value?.Date
                    : null,
                NormalizedValue = x.NormalizedValue,
                IsPotActive = x.IsActive
            })
            .ToList();

        ConversionRates = presentPotsResponse.ConversionRates
            .Select(x => new ExchangeRateViewModel(x, x.Date == presentPotsResponse.Date))
            .ToList();

        Total = presentPotsResponse.Total;
        CurrencyTotalOverviews = presentPotsResponse.CurrencyTotalOverviews;
    }
}