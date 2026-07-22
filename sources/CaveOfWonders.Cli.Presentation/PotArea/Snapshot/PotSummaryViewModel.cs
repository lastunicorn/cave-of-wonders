using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Snapshot;

internal class PotSummaryViewModel
{
	public Guid Id { get; }

	public string Name { get; }

	public string Currency { get; }

	public bool IsActive { get; }

	public PotSummaryViewModel(PotSummary potSummary)
	{
		Id = potSummary.Id;
		Name = potSummary.Name;
		Currency = potSummary.Currency;
		IsActive = potSummary.IsActive;
	}
}
