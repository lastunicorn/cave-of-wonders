namespace DustInTheWind.CaveOfWonders.Domain;

public record class GemParameter
{
	public int Id { get; set; }

	public Guid GemId { get; set; }

	public Gem Gem { get; set; }

	public string Key { get; set; }

	public string Value { get; set; }

	public virtual bool Equals(GemParameter other)
	{
		if (other == null) return false;

		return Key == other.Key && Value == other.Value;
	}
}