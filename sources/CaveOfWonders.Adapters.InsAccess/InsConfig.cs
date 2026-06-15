using Microsoft.Extensions.Configuration;

namespace DustInTheWind.CaveOfWonders.Adapters.InsAccess;

internal class InsConfig
{
    private const string AppSettingsFileName = "appsettings.json";
    private readonly IConfigurationSection insConfig;

    public Uri CpiPageUrl
    {
        get
        {
            string rawValue = insConfig["CpiPageUrl"];
            return Uri.TryCreate(rawValue, UriKind.Absolute, out Uri result)
                ? result
                : null;
        }
    }

    public InsConfig()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile(AppSettingsFileName, optional: true, reloadOnChange: true);

        IConfigurationRoot configurationRoot = builder.Build();

        insConfig = configurationRoot.GetRequiredSection("Ins");
    }
}