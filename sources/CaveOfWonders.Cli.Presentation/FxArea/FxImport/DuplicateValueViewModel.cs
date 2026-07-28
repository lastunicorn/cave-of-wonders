using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

internal class DuplicateValueViewModel
{
	public DateOnly Date { get; }

	public string CurrencyPair { get; }

	public decimal Value1 { get; }

	public decimal Value2 { get; }

	internal DuplicateValueViewModel(DuplicateReportResponseDto duplicateReport)
	{
		Date = duplicateReport.Date;
		CurrencyPair = duplicateReport.CurrencyPair;
		Value1 = duplicateReport.Value1;
		Value2 = duplicateReport.Value2;
	}
}