using DustInTheWind.CaveOfWonders.Cli.Application.PresentPotLabels;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Label;

internal class LabelViewModel
{
	public List<PotLabelsViewModel> Pots { get; }

	public List<LabelPotsViewModel> Labels { get; }

	public LabelViewModel(PresentPotLabelsResponse response)
	{
		Pots = response.PotLabels?
			.Select(x => new PotLabelsViewModel
			{
				PotId = x.PotId,
				PotName = x.PotName,
				Labels = x.Labels,
				IsActive = x.IsActive
			})
			.ToList();

		Labels = response.LabelPots?
			.Select(x => new LabelPotsViewModel
			{
				Label = x.Label,
				PotNames = x.Pots
					.Select(y => y.PotName)
					.ToList()
			})
			.ToList();
	}
}