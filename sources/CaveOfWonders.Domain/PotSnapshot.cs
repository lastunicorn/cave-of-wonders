using System.Text;

namespace DustInTheWind.CaveOfWonders.Domain;

public sealed record class PotSnapshot
{
	public DateOnly Date { get; init; }

	public decimal Value { get; init; }

	public Pot Pot { get; set; }

	public bool Equals(PotSnapshot other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Date.Equals(other.Date) && Value.Equals(other.Value);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Date, Value);
	}

	private bool PrintMembers(StringBuilder builder)
	{
		string potName = Pot?.Name ?? "<null>";
		builder.Append($"Date = {Date}, Value = {Value}, Pot = {potName}");

		return true;
	}
}