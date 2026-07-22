using DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotImport;

[NamedCommand("snapshot-import", Description = "Imports pot snapshots from csv exported files of the sheets of my ods file.")]
internal class PotImportCommand : IConsoleCommand<PotImportViewModel>
{
	private readonly IMediator mediator;

	[NamedParameter("source-file", ShortName = 'f', Description = "The full path of the xlsx file.")]
	public string SourceFilePath { get; set; }

	[NamedParameter("mappings-file", ShortName = 'm', Description = "The full path of the mappings file. This file specify which column from the spreadsheet to be imported in which pot.")]
	public string MappingsFilePath { get; set; }

	[NamedParameter("overwrite", ShortName = 'x', IsMandatory = false, Description = "If specified, the entire pot will be cleared before importing the snapshots.")]
	public bool Overwrite { get; set; }

	public PotImportCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<PotImportViewModel> Execute()
	{
		ImportPotSnapshotsRequest request = new()
		{
			SourceFilePath = SourceFilePath,
			MappingsFilePath = MappingsFilePath,
			Overwrite = Overwrite
		};

		ImportPotSnapshotsResponse response = await mediator.Send(request);

		return new PotImportViewModel
		{
			Report = response.Report,
		};
	}
}