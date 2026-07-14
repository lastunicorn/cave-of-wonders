using System.Collections;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Utils;

internal class BucketCollection<TKey, TItem> : IEnumerable<TItem>
{
    private readonly Dictionary<TKey, List<TItem>> items = new();

    public int Count => items.Sum(x => x.Value.Count);

    public List<TItem> GetBucket(TKey key)
    {
        bool success = items.TryGetValue(key, out List<TItem> bucket);

        return success ? bucket : null;
    }

    public List<TItem> GetOrCreateBucket(TKey key)
    {
        bool success = items.TryGetValue(key, out List<TItem> bucket);

        if (!success)
        {
            bucket = new List<TItem>();
            items.Add(key, bucket);
        }

        return bucket;
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return items.Values.SelectMany(x => x).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}