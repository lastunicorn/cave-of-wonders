using DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

internal class LabelAddViewModel
{
	public string Label { get; }

	public List<LabelAddItemViewModel> Items { get; }

	internal LabelAddViewModel(AddPotLabelResponse response)
	{
		Label = response.Label;
		Items = response.Items
			.Select(x => new LabelAddItemViewModel(x))
			.ToList();
	}
}
