using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;

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
	
	public static async Task<Measurement> Action(string actionName, Func<Task> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		await action();
		stopwatch.Stop();

		return new Measurement
		{
			Title = actionName,
			Time = stopwatch.Elapsed
		};
	}

	public static async Task<Measurement<TResponse>> Action<TResponse>(string actionName, Func<Task<TResponse>> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResponse result = await action();
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

	public static async Task<Measurement> Action(Func<Task> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		await action();
		stopwatch.Stop();

		return new Measurement
		{
			Time = stopwatch.Elapsed
		};
	}

	public static async Task<Measurement<TResponse>> Action<TResponse>(Func<Task<TResponse>> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResponse response = await action();
		stopwatch.Stop();

		return new Measurement<TResponse>
		{
			Time = stopwatch.Elapsed,
			Result = response
		};
	}
}