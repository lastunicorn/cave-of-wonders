using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal static class GemExtensions
{
    public static JGem ToJGem(this Gem gem)
    {
        JGem jGem = new()
        {
            Id = gem.Id,
            ExternalId = gem.ExternalId,
            Date = gem.Date,
            Amount = gem.Amount,
            Description = gem.Description,
            Category = gem.Category.ToString()
        };

        foreach (GemParameter gemParameter in gem.Parameters)
            jGem.Parameters.Add(gemParameter.Key, gemParameter.Value);

        return jGem;
    }
}