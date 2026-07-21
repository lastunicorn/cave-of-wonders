namespace DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;

public static class MeasurementExtensions
{
	public static Measurement DisplayToConsole(this Measurement measurement)
	{
		if (measurement.Title != null)
			Console.WriteLine($"{measurement.Title}: {FormatDuration(measurement.Time)}.");
		else
			Console.WriteLine($"{FormatDuration(measurement.Time)}");

		return measurement;
	}

	public static Measurement<TResponse> DisplayToConsole<TResponse>(this Measurement<TResponse> measurement)
	{
		if (measurement.Title != null)
			Console.WriteLine($"{measurement.Title}: {FormatDuration(measurement.Time)}.");
		else
			Console.WriteLine($"{FormatDuration(measurement.Time)}");

		return measurement;
	}

	private static string FormatDuration(TimeSpan time)
	{
		if (time.TotalMilliseconds < 1000)
			return $"{time.TotalMilliseconds:F2} ms";

		if (time.TotalSeconds < 60)
			return $"{time.TotalSeconds:F2} s";

		if (time.TotalMinutes < 60)
			return $"{time.TotalMinutes:F2} min";

		if (time.TotalHours < 24)
			return $"{time.TotalHours:F2} h";

		return $"{time.TotalDays:F2} d";
	}

	public static Measurement OnFinished(this Measurement measurement, Action<Measurement> action)
	{
		action?.Invoke(measurement);
		return measurement;
	}

	public static Measurement<TResponse> OnFinished<TResponse>(this Measurement<TResponse> measurement, Action<Measurement<TResponse>, TResponse> action)
	{
		action?.Invoke(measurement, measurement.Result);
		return measurement;
	}

	public static TResponse Response<TResponse>(this Measurement<TResponse> measurement)
	{
		return measurement.Result;
	}

	public static async Task<Measurement<TResponse>> OnFinished<TResponse>(this Task<Measurement<TResponse>> task, Action<Measurement<TResponse>, TResponse> action)
	{
		Measurement<TResponse> measurement = await task;
		action?.Invoke(measurement, measurement.Result);
		
		return measurement;
	}

	public static async Task<TResponse> Response<TResponse>(this Task<Measurement<TResponse>> task)
	{
		Measurement<TResponse> measurement = await task;
		return measurement.Result;
	}
}