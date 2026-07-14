namespace CaveOfWonders.Tests;

internal static class AsyncEnumerableExtensions
{
	public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
	{
		foreach (T item in source)
			yield return item;
	}
}