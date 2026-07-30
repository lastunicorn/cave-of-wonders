using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class PotLabelsViewModel
{
	public Guid PotId { get; init; }

	public string PotName { get; init; }

	public List<string> Labels { get; init; }

	public bool IsActive { get; init; }
}