using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;

public record class Measurement
{
	public string Title { get; init; }

	public TimeSpan Time { get; init; }

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
}