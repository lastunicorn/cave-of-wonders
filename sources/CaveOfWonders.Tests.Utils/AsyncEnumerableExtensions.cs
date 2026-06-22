namespace DustInTheWind.CaveOfWonders.Tests.Utils;

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        List<T> list = [];

        await foreach (T item in source)
            list.Add(item);

        return list;
    }
}