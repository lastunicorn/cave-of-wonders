using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal static class JGemExtensions
{
    public static Gem ToGem(this JGem jGem)
    {
        Gem gem = new()
        {
            Id = jGem.Id,
            ExternalId = jGem.ExternalId,
            Date = jGem.Date,
            Amount = jGem.Amount,
            Description = jGem.Description
        };

        if (Enum.TryParse(jGem.Category, out GemCategory category))
            gem.Category = category;

        foreach (KeyValuePair<string, string> jGemParameter in jGem.Parameters)
            gem.Parameters.Add(new GemParameter
            {
                Key = jGemParameter.Key,
                Value = jGemParameter.Value
            });
                
        return gem;
    }
}