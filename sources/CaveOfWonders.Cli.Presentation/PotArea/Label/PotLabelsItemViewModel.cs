using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class PotLabelsItemViewModel
{
	public Guid PotId { get; }

	public string PotName { get; }

	public List<string> Labels { get; }

	public bool IsActive { get; }

	internal PotLabelsItemViewModel(PotLabelsItem item)
	{
		PotId = item.PotId;
		PotName = item.PotName;
		Labels = item.Labels;
		IsActive = item.IsActive;
	}
}
