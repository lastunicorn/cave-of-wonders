using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Inflation;

[NamedCommand("inflation", Description = "Display the inflation.")]
internal class InflationCommand : IConsoleCommand<InflationViewModel>
{
	private readonly RequestBus requestBus;

	public InflationCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<InflationViewModel> Execute()
	{
		PresentInflationRequest request = new();
		PresentInflationResponse response = await requestBus.SendAsync<PresentInflationRequest, PresentInflationResponse>(request);

		return new InflationViewModel
		{
			Records = response.InflationRecords
		};
	}
}