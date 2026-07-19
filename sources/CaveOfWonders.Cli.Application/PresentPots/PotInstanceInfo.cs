namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PotInstanceInfo
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public CurrencyValue Value { get; set; }

	public CurrencyValue NormalizedValue { get; set; }

	public bool IsActive { get; set; }
}