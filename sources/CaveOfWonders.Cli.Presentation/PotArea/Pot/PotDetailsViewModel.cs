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

    public int SnapshotCount { get; }

    public DateOnly? LastSnapshotDate { get; }

    public CurrencyValue Value { get; }
    
    public List<string> Labels { get; }

    public bool IsActive { get; }

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
        SnapshotCount = potDetails.SnapshotCount;
        LastSnapshotDate = potDetails.LastSnapshotDate;
        Value = potDetails.Value;
        Labels = potDetails.Labels.ToList();
        IsActive = potDetails.IsActive;
    }
}