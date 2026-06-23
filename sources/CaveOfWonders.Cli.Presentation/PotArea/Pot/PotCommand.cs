using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

[NamedCommand("pot", Description = "Display details about a specific pot.")]
internal class PotCommand : IConsoleCommand<PotCommandViewModel>
{
    private readonly IMediator mediator;

    [AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsMandatory = false, Description = "Name or id of the pot. Partial id is accepted.")]
    public string PotIdentifier { get; set; }

    [NamedParameter("all", ShortName = 'a', IsMandatory = false, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    [NamedParameter("details", ShortName = 'd', IsMandatory = false, Description = "Display details about the pot. Default = false.")]
    public bool? ShowDetails { get; set; }

    public PotCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<PotCommandViewModel> Execute()
    {
        PresentPotRequest request = new()
        {
            PotFlexId = PotIdentifier,
            IncludeInactivePots = IncludeInactivePots,
            ShowDetails = ShowDetails
        };

        PresentPotResponse response = await mediator.Send(request);

        return new PotCommandViewModel
        {
            PotDetails = response.PotDetails?
                .Select(x => new PotDetailsViewModel(x))
                .ToList(),
            PotSummaries = response.PotSummaries?
                .Select(x => new PotSummaryViewModel(x))
                .ToList()
        };
    }
}