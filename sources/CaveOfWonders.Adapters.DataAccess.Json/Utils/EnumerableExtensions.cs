using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.Utils;

internal static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        T[] bucket = null;
        int count = 0;

        foreach (T item in source)
        {
            bucket ??= new T[size];

            bucket[count++] = item;

            if (count != size)
                continue;

            yield return bucket;

            bucket = null;
            count = 0;
        }

        if (bucket != null && count > 0)
        {
            Array.Resize(ref bucket, count);
            yield return bucket.Select(x => x);
        }
    }
    
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (T item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }
}