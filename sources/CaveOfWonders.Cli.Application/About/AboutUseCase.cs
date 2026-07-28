using System.Reflection;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.About;

internal class AboutUseCase : IUseCase<AboutRequest, AboutResponse>
{
    private readonly IDatabaseConfiguration databaseConfiguration;

    public AboutUseCase(IDatabaseConfiguration databaseConfiguration)
    {
        this.databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
    }

    public Task<AboutResponse> Execute(AboutRequest request, CancellationToken cancellationToken)
    {
        Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

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
