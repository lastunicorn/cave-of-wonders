using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pot;

internal class PotDetailsViewModel
{
    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateOnly StartDate { get; }

    public DateOnly? EndDate { get; }

    public string Currency { get; }

    public int GemCount { get; }

    public bool IsActive { get; set; }

    public DateOnly? LastGemDate { get; }

    public CurrencyValue Value { get; }
    
    public List<string> Labels { get; }

    public PotDetailsViewModel(PotDetails potDetails)
    {
        if (potDetails == null)
            return;

        Id = potDetails.Id;
        Name = potDetails.Name;
        Description = potDetails.Description;
        StartDate = potDetails.StartDate;
        EndDate = potDetails.EndDate;
        Currency = potDetails.Currency;
        GemCount = potDetails.GemCount;
        LastGemDate = potDetails.LastGemDate;
        Value = potDetails.Value;
        Labels = potDetails.Labels.ToList();

        IsActive = potDetails.EndDate == null || potDetails.EndDate >= DateOnly.FromDateTime(DateTime.Today);
    }
}