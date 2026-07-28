using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gain;

[NamedCommand("gain", Description = "Display the gain obtained in a given period.")]
internal class GainCommand : IConsoleCommand<GainViewModel>
{
	private readonly RequestBus requestBus;

	[NamedParameter("month", ShortName = 'm', IsMandatory = false, Description = "The month for which to calculate the gain. Default = current month.")]
	public string Month { get; set; }

	public GainCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<GainViewModel> Execute()
	{
		GainRequest request = new()
		{
			Month = Month
		};
		GainResponse response = await requestBus.SendAsync<GainRequest, GainResponse>(request);

		return new GainViewModel(response);
	}
}