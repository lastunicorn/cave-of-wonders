using DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Cpi;

[NamedCommand("cpi", Description = "Display the consumer price indexes.")]
internal class CpiCommand : IConsoleCommand<CpiViewModel>
{
	private readonly RequestBus requestBus;

	public CpiCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<CpiViewModel> Execute()
	{
		PresentCpiRequest request = new();
		PresentCpiResponse response = await requestBus.SendAsync<PresentCpiRequest, PresentCpiResponse>(request);

		return new CpiViewModel
		{
			Records = response.InflationRecords
		};
	}
}