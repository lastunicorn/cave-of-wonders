using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Wealth;

internal class WealthViewModel
{
    public CultureInfo Culture { get; set; }

    public DateOnly Date { get; }

    public List<PotSnapshotViewModel> Values { get; }

    public List<ExchangeRateViewModel> ConversionRates { get; }

    public DatedAmount Total { get; }

    public List<CurrencyTotalOverview> CurrencyTotalOverviews { get; }

    public WealthViewModel(PresentWealthResponse presentWealthResponse)
    {
        Date = presentWealthResponse.Date;

        Values = presentWealthResponse.PotInstances
            .Select(x => new PotSnapshotViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OriginalValue = x.IsActive
                    ? x.Value
                    : null,
                IsValueActual = x.Value?.Date == presentWealthResponse.Date,
                IsValueAlreadyNormal = x.Value?.Currency == x.NormalizedValue?.Currency,
                IsNormalizedCurrent = x.NormalizedValue?.Date == Date,
                Date = x.IsActive
                    ? x.Value?.Date
                    : null,
                NormalizedValue = x.NormalizedValue,
                IsPotActive = x.IsActive
            })
            .ToList();

        ConversionRates = presentWealthResponse.ConversionRates
            .Select(x => new ExchangeRateViewModel(x, x.Date == presentWealthResponse.Date))
            .ToList();

        Total = presentWealthResponse.Total;
        CurrencyTotalOverviews = presentWealthResponse.CurrencyTotalOverviews;
    }
}