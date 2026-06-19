using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

internal class GemCommandViewModel
{
    public List<GemDto> Gems { get; set; }

    public decimal CalculateTotal()
    {
        if(Gems.Count == 0)
            return 0;
        
        return Gems
            .Where(x => x.Category is GemCategory.Gain or GemCategory.Loss)
            .Sum(x =>
            {
                return x.Category switch
                {
                    GemCategory.Gain => x.Amount,
                    GemCategory.Loss => -x.Amount,
                    _ => 0
                };
            });
    }
}