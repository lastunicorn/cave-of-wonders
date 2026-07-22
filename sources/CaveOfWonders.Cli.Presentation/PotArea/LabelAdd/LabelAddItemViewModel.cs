using DustInTheWind.CaveOfWonders.Cli.Application.AddPotLabel;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.LabelAdd;

internal class LabelAddItemViewModel
{
	public Guid PotId { get; }

	public string PotName { get; }

	public bool WasAdded { get; }

	public bool IsActive { get; }

	internal LabelAddItemViewModel(LabelAddResult item)
	{
		PotId = item.PotId;
		PotName = item.PotName;
		WasAdded = item.WasAdded;
		IsActive = item.IsActive;
	}
}
