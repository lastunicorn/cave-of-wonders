using DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Cpi;

[NamedCommand("cpi", Description = "Display the consumer price indexes.")]
internal class CpiCommand : IConsoleCommand<CpiViewModel>
{
	private readonly IMediator mediator;

	public CpiCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<CpiViewModel> Execute()
	{
		PresentCpiRequest request = new();
		PresentCpiResponse response = await mediator.Send(request);

		return new CpiViewModel
		{
			Records = response.InflationRecords
		};
	}
}