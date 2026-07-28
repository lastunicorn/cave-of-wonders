using DustInTheWind.CaveOfWonders.Cli.Application.PresentWage;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.Wage;

[NamedCommand("wage", Description = "Displays average wage information.")]
internal class WageCommand : IConsoleCommand<WagesViewModel>
{
	private readonly RequestBus requestBus;

	public WageCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<WagesViewModel> Execute()
	{
		PresentWageRequest request = new();
		PresentWageResponse response = await requestBus.SendAsync<PresentWageRequest, PresentWageResponse>(request);

		return new WagesViewModel
		{
			Wages = response.Values
				.Select(x => new WageViewModel
				{
					Year = x.Year,
					GrossValue = x.GrossValue,
					NetValue = x.NetValue
				})
				.ToList()
		};
	}
}