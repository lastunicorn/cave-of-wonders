namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PotInstanceInfo
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public DatedAmount Value { get; set; }

	public DatedAmount NormalizedValue { get; set; }

	public bool IsActive { get; set; }
}