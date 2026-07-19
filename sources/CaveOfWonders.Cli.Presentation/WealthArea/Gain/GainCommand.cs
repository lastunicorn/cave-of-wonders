using DustInTheWind.CaveOfWonders.Cli.Application.CalculateGain;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Gain;

[NamedCommand("gain", Description = "Display the gain obtained in a given period.")]
internal class GainCommand : IConsoleCommand<GainViewModel>
{
	private readonly IMediator mediator;

	[NamedParameter("month", ShortName = 'm', IsMandatory = false, Description = "The month for which to calculate the gain. Default = current month.")]
	public string Month { get; set; }

	public GainCommand(IMediator mediator)
	{
		this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	public async Task<GainViewModel> Execute()
	{
		GainRequest request = new()
		{
			Month = Month
		};
		GainResponse response = await mediator.Send(request);

		return new GainViewModel
		{
			Items = response.Items,
			TotalGain = response.TotalGain
		};
	}
}