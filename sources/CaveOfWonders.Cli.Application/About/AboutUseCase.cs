using System.Reflection;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.About;

internal class AboutUseCase : IRequestHandler<AboutRequest, AboutResponse>
{
    private readonly IDatabaseConfiguration databaseConfiguration;

    public AboutUseCase(IDatabaseConfiguration databaseConfiguration)
    {
        this.databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
    }

    public Task<AboutResponse> Handle(AboutRequest request, CancellationToken cancellationToken)
    {
        Assembly assembly = typeof(AboutUseCase).Assembly;

        string applicationName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        string author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        string description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        string version = assembly.GetName().Version?.ToString();

        AboutResponse response = new()
        {
            ApplicationName = applicationName,
            Version = version,
            Author = author,
            Description = description,
            DatabaseLocation = databaseConfiguration.ConnectionString
        };

        return Task.FromResult(response);
    }
}
