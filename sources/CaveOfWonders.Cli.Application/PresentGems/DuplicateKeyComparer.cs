namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

/// <summary>
/// Comparer that allows duplicate keys in a sorted list by treating equal keys as greater than each other.
/// </summary>
internal class DuplicateKeyComparer<TKey> : IComparer<TKey>
	where TKey : IComparable
{
	public int Compare(TKey x, TKey y)
	{
		if (x == null)
			return -1;

		if (y == null)
			return 1;

		int result = x.CompareTo(y);

		if (result == 0)
			return 1; // Handle equality as being greater. Note: this will break Remove(key) or IndexOfKey(key) since the comparer never returns 0 to signal key equality

		return result;
	}
}