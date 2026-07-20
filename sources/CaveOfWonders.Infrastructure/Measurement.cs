namespace DustInTheWind.CaveOfWonders.Infrastructure;

public record class Measurement
{
	public string Title { get; init; }

	public TimeSpan Time { get; init; }
}