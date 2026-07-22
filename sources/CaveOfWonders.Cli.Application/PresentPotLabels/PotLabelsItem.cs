namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

public class PotLabelsItem
{
	public Guid PotId { get; init; }

	public string PotName { get; init; }

	public List<string> Labels { get; init; } = [];
}
