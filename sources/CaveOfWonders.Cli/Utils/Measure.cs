using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Cli.Utils;

internal static class Measure
{
	public static void Action(string actionName, Action action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();

		Console.WriteLine($"{actionName}: {stopwatch.ElapsedMilliseconds} ms.");
	}

	public static T Action<T>(string actionName, Func<T> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		T result = action();
		stopwatch.Stop();

		Console.WriteLine($"{actionName}: {stopwatch.ElapsedMilliseconds} ms.");

		return result;
	}
}