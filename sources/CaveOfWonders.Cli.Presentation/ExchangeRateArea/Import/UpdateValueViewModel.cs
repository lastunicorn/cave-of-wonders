using DustInTheWind.CaveOfWonders.Cli.Application.ImportFromBnrFile;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Import;

internal class UpdateValueViewModel
{
    public DateTime Date { get; }

    public string CurrencyPair { get; }

    public decimal OldValue { get; }

    public decimal NewValue { get; }

    internal UpdateValueViewModel(UpdateReportResponseDto updateReport)
    {
        Date = updateReport.Date;
        CurrencyPair = updateReport.CurrencyPair;
        OldValue = updateReport.OldValue;
        NewValue = updateReport.NewValue;
    }
}