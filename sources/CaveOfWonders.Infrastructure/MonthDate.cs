namespace DustInTheWind.CaveOfWonders.Infrastructure;

public readonly record struct MonthDate
{
	public int Year { get; private init; }

	public int Month { get; private init; }

	public bool HasValue => Year > 0 && Month > 0;

	public MonthDate(int year, int month)
	{
		if (year <= 0)
			throw new ArgumentOutOfRangeException("Invalid year.");

		if (month is <= 0 or > 12)
			throw new ArgumentOutOfRangeException("Invalid month.");

		Year = year;
		Month = month;
	}

	public MonthDate(DateOnly date)
		: this(date.Year, date.Month)
	{
	}

	public bool Contains(DateTime date)
	{
		return date.Year == Year && date.Month == Month;
	}

	public override string ToString()
	{
		return $"{Month:00}/{Year}";
	}

	public static MonthDate Parse(string text)
	{
		if (text == null)
			return default;

		string[] parts = text.Split('/');

		if (parts.Length != 2)
			throw new FormatException("Invalid month date format.");

		return new MonthDate
		{
			Month = int.Parse(parts[0]),
			Year = int.Parse(parts[1])
		};
	}

	public static bool TryParse(string text, out MonthDate monthDate)
	{
		monthDate = default;

		if (text == null)
			return false;

		string[] parts = text.Split('/');

		if (parts.Length != 2)
			return false;

		if (!int.TryParse(parts[0], out int month))
			return false;

		if (!int.TryParse(parts[1], out int year))
			return false;

		monthDate = new MonthDate
		{
			Month = month,
			Year = year
		};

		return true;
	}

	public static implicit operator string(MonthDate monthDate)
	{
		return monthDate.ToString();
	}

	public static implicit operator MonthDate(string monthDate)
	{
		return Parse(monthDate);
	}
}