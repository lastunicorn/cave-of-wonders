using DustInTheWind.CaveOfWonders.Cli.Application.ExportInflation;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.InflationExport;

[NamedCommand("inflation-export", Description = "Exports the inflation information to a file on disk.")]
internal class InflationExportCommand : IConsoleCommand
{
	private readonly IMediator mediator;

	[NamedParameter("output", ShortName = 'o', IsMandatory = false, Description = "Path to the output file.")]
	public string OutputPath { get; set; }

	public InflationExportCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task Execute()
	{
		ExportInflationRequest request = new()
		{
			OutputPath = OutputPath
		};
		await mediator.Send(request);
	}
}