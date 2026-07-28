using System.Reflection;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.About;

internal class AboutUseCase : IRequestHandler<AboutRequest, AboutResponse>
{
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
            Description = description
        };

        return Task.FromResult(response);
    }
}
