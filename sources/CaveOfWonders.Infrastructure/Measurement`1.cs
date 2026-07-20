
namespace DustInTheWind.CaveOfWonders.Infrastructure;

public record class Measurement<TResponse>
{
	public string Title { get; init; }

	public TimeSpan Time { get; init; }

	public TResponse Result { get; init; }
}