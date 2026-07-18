using DustInTheWind.CaveOfWonders.Cli.Presentation;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Setup.Microsoft;

namespace DustInTheWind.CaveOfWonders.Cli;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
            .ConfigureServices(DependenciesSetup.Configure)
            .RegisterCommandsFrom(typeof(PresentationAssemblyHandle).Assembly)
            .HandleExceptions(HandlerApplicationException)
            .Build();

        await application.RunAsync(args);
    }

    private static void HandlerApplicationException(object o, UnhandledApplicationExceptionEventArgs e)
    {
        CustomConsole.WriteError(e.Exception);
    }
}