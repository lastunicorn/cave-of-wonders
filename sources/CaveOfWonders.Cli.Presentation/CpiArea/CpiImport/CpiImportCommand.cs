using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.CpiImport;

[NamedCommand("cpi-import", Description = "Imports Romanian annual inflation values from the INS web page or a file.")]
internal class CpiImportCommand : IConsoleCommand<CpiImportViewModel>
{
	private readonly RequestBus requestBus;

	[NamedParameter("source-type", ShortName = 't', IsMandatory = false, Description = "The source of the imported data. (file - text file; web - html webpage from https://insse.ro)")]
	public CpiImportSourceType ImportSourceType { get; set; } = CpiImportSourceType.Web;

	[NamedParameter("source-file", ShortName = 'f', IsMandatory = false)]
	public string SourceFile { get; set; }

	public CpiImportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<CpiImportViewModel> Execute()
	{
		ImportCpiRequest request = new()
		{
			ImportSource = ImportSourceType switch
			{
				CpiImportSourceType.File => ImportSource.File,
				CpiImportSourceType.Web => ImportSource.Web,
				_ => throw new ArgumentOutOfRangeException()
			},
			SourceFilePath = SourceFile
		};
		ImportCpiResponse response = await requestBus.SendAsync<ImportCpiRequest, ImportCpiResponse>(request);

		return new CpiImportViewModel
		{
			AddedCount = response.AddedCount,
			UpdatedCount = response.UpdatedCount,
			TotalCount = response.TotalCount
		};
	}
}