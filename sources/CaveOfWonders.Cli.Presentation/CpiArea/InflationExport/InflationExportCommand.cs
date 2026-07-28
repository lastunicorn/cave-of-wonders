using DustInTheWind.CaveOfWonders.Cli.Application.ExportInflation;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.InflationExport;

[NamedCommand("inflation-export", Description = "Exports the inflation information to a file on disk.")]
internal class InflationExportCommand : IConsoleCommand
{
	private readonly RequestBus requestBus;

	[NamedParameter("output", ShortName = 'o', IsMandatory = false, Description = "Path to the output file.")]
	public string OutputPath { get; set; }

	public InflationExportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task Execute()
	{
		ExportInflationRequest request = new()
		{
			OutputPath = OutputPath
		};
		await requestBus.SendAsync<ExportInflationRequest>(request);
	}
}