using System.Collections.ObjectModel;

namespace DustInTheWind.CaveOfWonders.Domain;

public class PotSnapshotCollection : Collection<PotSnapshot>
{
    private readonly Pot pot;

    public PotSnapshotCollection(Pot pot)
    {
        this.pot = pot ?? throw new ArgumentNullException(nameof(pot));
    }

    public void AddRange(IEnumerable<PotSnapshot> potSnapshots)
    {
        ArgumentNullException.ThrowIfNull(potSnapshots);

        foreach (PotSnapshot potSnapshot in potSnapshots)
            Add(potSnapshot);
    }

    protected override void InsertItem(int index, PotSnapshot item)
    {
        ArgumentNullException.ThrowIfNull(item);

        base.InsertItem(index, item);
        item.Pot = pot;
    }

    protected override void SetItem(int index, PotSnapshot item)
    {
        ArgumentNullException.ThrowIfNull(item);

        PotSnapshot oldItem = this[index];

        base.SetItem(index, item);

        if (oldItem != item)
            oldItem.Pot = null;

        item.Pot = pot;
    }

    protected override void RemoveItem(int index)
    {
        PotSnapshot oldItem = this[index];

        base.RemoveItem(index);

        if (oldItem != null)
            oldItem.Pot = null;
    }

    protected override void ClearItems()
    {
        foreach (PotSnapshot potSnapshot in this)
            potSnapshot.Pot = null;

        base.ClearItems();
    }
}
