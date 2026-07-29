using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;

public record class Measurement<TResponse>
{
	public string Title { get; init; }

	public TimeSpan Time { get; init; }

	public TResponse Result { get; init; }

	public static Measurement<TResponse> Action(string actionName, Func<TResponse> action)
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

	public static async Task<Measurement<TResponse>> Action(string actionName, Func<Task<TResponse>> action)
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

	public static Measurement<TResponse> Action(Func<TResponse> action)
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

	public static async Task<Measurement<TResponse>> Action(Func<Task<TResponse>> action)
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