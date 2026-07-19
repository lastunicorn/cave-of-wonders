using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport;

[NamedCommand("gem-import", Description = "Import gems.")]
internal class GemImportCommand : IConsoleCommand<GemImportViewModel>
{
	private readonly IMediator mediator;

	[NamedParameter("file", IsMandatory = true, Description = "The path to the file from which to import the gems.")]
	public string FilePath { get; set; }

	[NamedParameter("file-type", IsMandatory = true, Description = "The type of the file to import. Supported values: 'mintos', 'fintown', 'bcr'.")]
	public FileType FileType { get; set; }

	[NamedParameter("pot", IsMandatory = false, Description = "The pot id for which to import the gems.")]
	public string PotId { get; set; }

	public GemImportCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<GemImportViewModel> Execute()
	{
		ImportGemsRequest request = new()
		{
			FilePath = FilePath,
			FileType = FileType,
			PotFlexId = PotId
		};

		ImportGemsResponse response = await mediator.Send(request);

		return new GemImportViewModel
		{
			UpdatedGemCount = response.UpdatedGemCount,
			AddedGemCount = response.AddedGemCount,
			SkippedGemCount = response.SkippedGemCount,
			TotalGemCount = response.TotalGemCount
		};
	}
}