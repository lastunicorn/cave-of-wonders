using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class LabelViewModel
{
	public List<PotLabelsItemViewModel> Items { get; }

	internal LabelViewModel(PresentPotLabelsResponse response)
	{
		Items = response.Items
			.Select(x => new PotLabelsItemViewModel(x))
			.ToList();
	}
}
