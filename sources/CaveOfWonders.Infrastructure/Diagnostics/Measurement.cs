namespace DustInTheWind.CaveOfWonders.Infrastructure.Diagnostics;

public record class Measurement
{
	public string Title { get; init; }

	public TimeSpan Time { get; init; }
}