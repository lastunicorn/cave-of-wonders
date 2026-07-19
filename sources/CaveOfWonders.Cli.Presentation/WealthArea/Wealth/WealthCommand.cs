using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;
using System.Globalization;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WealthArea.Wealth;

[NamedCommand("wealth", Description = "Display an overview of the entire cave (the wealth).")]
internal class WealthCommand : IConsoleCommand<WealthViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("date", ShortName = 'd', IsMandatory = false, Description = "The date for which to display the state of the cave. Default value = today")]
    public DateOnly? Date { get; set; }

    [NamedParameter("currency", ShortName = 'c', IsMandatory = false)]
    public string Currency { get; set; }

    [NamedParameter("all", ShortName = 'a', IsMandatory = false, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    [NamedParameter("culture", ShortName = 'u', IsMandatory = false, Description = "The culture info used for displaying the data.")]
    public CultureInfo Culture { get; set; }

    public WealthCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<WealthViewModel> Execute()
    {
        PresentPotsRequest request = new()
        {
            Date = Date,
            Currency = Currency,
            IncludeInactive = IncludeInactivePots
        };

        PresentPotsResponse response = await mediator.Send(request);

        return new WealthViewModel(response)
        {
            Culture = Culture,
        };
    }
}