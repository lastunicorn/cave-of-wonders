namespace DustInTheWind.CaveOfWonders.Infrastructure;

internal static class EnumerableExtensions
{
    public static IEnumerable<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source
            .Select(selector)
            .ToList();
    }
}