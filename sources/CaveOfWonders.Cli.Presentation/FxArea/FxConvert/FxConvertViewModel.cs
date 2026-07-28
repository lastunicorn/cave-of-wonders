using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxConvert;

internal class FxConvertViewModel
{
	public decimal InitialValue { get; set; }

	public decimal ConvertedValue { get; set; }

	public string SourceCurrency { get; set; }

	public string DestinationCurrency { get; set; }

	public ExchangeRateViewModel ExchangeRate { get; set; }
}