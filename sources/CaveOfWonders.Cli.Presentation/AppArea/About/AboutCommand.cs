using DustInTheWind.CaveOfWonders.Cli.Application.About;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.AppArea.About;

[NamedCommand("about", Description = "Displays information about the application.")]
internal class AboutCommand : IConsoleCommand<AboutViewModel>
{
    private readonly IMediator mediator;

    public AboutCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<AboutViewModel> Execute()
    {
        AboutRequest request = new();
        AboutResponse response = await mediator.Send(request);

        return new AboutViewModel
        {
            ApplicationName = response.ApplicationName,
            Version = response.Version,
            Author = response.Author,
            Description = response.Description
        };
    }
}
