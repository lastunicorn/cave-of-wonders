using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

internal class UpdateValueViewModel
{
	public DateOnly Date { get; }

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