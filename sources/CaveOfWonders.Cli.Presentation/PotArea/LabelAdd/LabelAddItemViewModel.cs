using DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

internal class LabelAddItemViewModel
{
	public Guid PotId { get; }

	public string PotName { get; }

	internal LabelAddItemViewModel(LabelAddResult item)
	{
		PotId = item.PotId;
		PotName = item.PotName;
	}
}
