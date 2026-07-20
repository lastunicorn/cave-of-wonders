using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Domain;

public record class Gem
{
	public Guid Id { get; set; }

	public string ExternalId { get; set; }

	public DateTime Date { get; set; }

	public GemCategory Category { get; set; }

	public decimal Amount { get; set; }

	public string Description { get; set; }

	public GemParameterCollection Parameters { get; }

	public Pot Pot { get; set; }

	public Gem()
	{
		Parameters = new GemParameterCollection(this);
	}

	public string GetParameterValue(string parameterName)
	{
		return Parameters
			.FirstOrDefault(x => x.Key == parameterName)?
			.Value;
	}

	public bool HasParameter(string key, string value)
	{
		return Parameters
			.Any(x => x.Key == key && x.Value == value);
	}

	public virtual bool Equals(Gem other)
	{
		if (other == null) return false;

		return Date == other.Date
			&& Category == other.Category
			&& Amount == other.Amount
			&& Description == other.Description
			&& Parameters.SequenceEqual(other.Parameters)
			&& Pot?.Id == other.Pot?.Id;
	}
}