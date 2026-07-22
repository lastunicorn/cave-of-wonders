using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Inflation;

[NamedCommand("inflation", Description = "Display the inflation.")]
internal class InflationCommand : IConsoleCommand<InflationViewModel>
{
	private readonly IMediator mediator;

	public InflationCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<InflationViewModel> Execute()
	{
		PresentInflationRequest request = new();
		PresentInflationResponse response = await mediator.Send(request);

		return new InflationViewModel
		{
			Records = response.InflationRecords
		};
	}
}