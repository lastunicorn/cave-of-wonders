using DustInTheWind.CaveOfWonders.Cli.Application.ImportAverageWage;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.WageImport;

[NamedCommand("wage-import", Description = "Imports the average wage data from the INS web page.")]
internal class WageImportCommand : IConsoleCommand<WageImportViewModel>
{
	private readonly RequestBus requestBus;

	public WageImportCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<WageImportViewModel> Execute()
	{
		WageImportRequest request = new();
		WageImportResponse response = await requestBus.SendAsync<WageImportRequest, WageImportResponse>(request);

		return new WageImportViewModel
		{
			TotalCount = response.TotalCount,
			AddedCount = response.AddedCount,
			UpdatedCount = response.UpdatedCount,
			DeletedCount = response.DeletedCount
		};
	}
}