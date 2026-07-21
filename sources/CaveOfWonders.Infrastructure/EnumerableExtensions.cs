using System.Runtime.CompilerServices;

namespace DustInTheWind.CaveOfWonders.Infrastructure;

public static class EnumerableExtensions
{
	public static IEnumerable<TResult> ToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		return source
			.Select(selector)
			.ToList();
	}

	public static async IAsyncEnumerable<TResult> ToAsyncEnumerable<TResult>(this IEnumerable<TResult> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (TResult result in source)
		{
			cancellationToken.ThrowIfCancellationRequested();
			yield return result;
		}
	}
}