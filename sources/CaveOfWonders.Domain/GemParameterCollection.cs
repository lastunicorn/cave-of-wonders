using System.Collections.ObjectModel;

namespace DustInTheWind.CaveOfWonders.Domain;

public class GemParameterCollection : Collection<GemParameter>
{
	private readonly Gem gem;

	public GemParameterCollection(Gem gem)
	{
		this.gem = gem ?? throw new ArgumentNullException(nameof(gem));
	}

	public void AddRange(IEnumerable<GemParameter> gemParameters)
	{
		ArgumentNullException.ThrowIfNull(gemParameters);

		foreach (GemParameter gemParameter in gemParameters)
			Add(gemParameter);
	}

	protected override void InsertItem(int index, GemParameter item)
	{
		ArgumentNullException.ThrowIfNull(item);

		base.InsertItem(index, item);
		item.Gem = gem;
	}

	protected override void SetItem(int index, GemParameter item)
	{
		ArgumentNullException.ThrowIfNull(item);

		GemParameter oldItem = this[index];

		base.SetItem(index, item);

		if (oldItem != item)
			oldItem.Gem = null;

		item.Gem = gem;
	}

	protected override void RemoveItem(int index)
	{
		GemParameter oldItem = this[index];

		base.RemoveItem(index);

		if (oldItem != null)
			oldItem.Gem = null;
	}

	protected override void ClearItems()
	{
		foreach (GemParameter gemParameter in this)
			gemParameter.Gem = null;

		base.ClearItems();
	}
}