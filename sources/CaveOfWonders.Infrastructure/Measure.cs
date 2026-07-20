using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Infrastructure;

public static class Measure
{
	public static Measurement Action(string actionName, Action action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();

		return new Measurement
		{
			Title = actionName,
			Time = stopwatch.Elapsed
		};
	}

	public static Measurement<TResponse> Action<TResponse>(string actionName, Func<TResponse> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResponse result = action();
		stopwatch.Stop();

		return new Measurement<TResponse>
		{
			Title = actionName,
			Time = stopwatch.Elapsed,
			Result = result
		};
	}

	public static Measurement Action(Action action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();

		return new Measurement
		{
			Time = stopwatch.Elapsed
		};
	}

	public static Measurement<TResponse> Action<TResponse>(Func<TResponse> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResponse response = action();
		stopwatch.Stop();

		return new Measurement<TResponse>
		{
			Time = stopwatch.Elapsed,
			Result = response
		};
	}
}