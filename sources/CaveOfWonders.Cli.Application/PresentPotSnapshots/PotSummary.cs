using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotSnapshots;

public class PotSummary
{
	public Guid Id { get; }

	public string Name { get; }

	public string Currency { get; }

	public bool IsActive { get; }

	public PotSummary(Pot pot, bool isActive)
	{
		Id = pot.Id;
		Name = pot.Name;
		Currency = pot.Currency;
		IsActive = isActive;
	}
}
