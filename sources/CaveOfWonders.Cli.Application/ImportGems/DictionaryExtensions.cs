namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

internal static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> destination, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        foreach (KeyValuePair<TKey, TValue> item in items)
            destination.Add(item.Key, item.Value);
    }
}