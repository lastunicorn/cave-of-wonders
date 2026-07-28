using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

[NamedCommand("fx-import", Description = "Execute exchange rates from local files or directly from the BNR website.")]
internal class FxImportCommand : IConsoleCommand<FxImportViewModel>
{
	private readonly RequestBus requestBus;

	[NamedParameter("source-type", ShortName = 's', IsMandatory = true, Description = "The source of the imported data. (nbr - nbr file; web - nbr file from BNR website)")]
	public ImportSourceType SourceType { get; set; }

	[NamedParameter("file", ShortName = 'f', IsMandatory = false, Description = "The full name of the file containing the exchange rates. Used by bnr and nbr imports.")]
	public string SourceFilePath { get; set; }

	[NamedParameter("year", ShortName = 'y', IsMandatory = false, Description = "The year to be imported. Used by web import.")]
	public int? Year { get; set; }

	public FxImportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<FxImportViewModel> Execute()
	{
		ImportExchangeRatesRequest request = new()
		{
			ImportSource = SourceType switch
			{
				ImportSourceType.Nbr => ImportSource.BnrNbrFile,
				ImportSourceType.Web => ImportSource.BnrWebsite,
				_ => throw new ArgumentOutOfRangeException()
			},
			SourceFilePath = SourceFilePath,
			Year = Year
		};

		ImportExchangeRatesResponse response = await requestBus.SendAsync<ImportExchangeRatesRequest, ImportExchangeRatesResponse>(request);

		return new FxImportViewModel(response);
	}
}