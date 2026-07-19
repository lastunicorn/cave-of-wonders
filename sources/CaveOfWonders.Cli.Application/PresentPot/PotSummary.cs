using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

public class PotSummary
{
	public PotSummary(Pot pot)
	{
		Id = pot.Id;
		Name = pot.Name;
		Currency = pot.Currency;
		IsActive = pot.EndDate == null || pot.EndDate >= DateOnly.FromDateTime(DateTime.Today);
	}

	public Guid Id { get; }

	public string Name { get; }

	public string Currency { get; }

	public bool IsActive { get; }
}