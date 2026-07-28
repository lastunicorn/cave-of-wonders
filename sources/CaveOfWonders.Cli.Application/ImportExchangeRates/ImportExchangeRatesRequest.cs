namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

public class ImportExchangeRatesRequest
{
	public ImportSource ImportSource { get; set; }

	public string SourceFilePath { get; set; }

	public int? Year { get; set; }
}