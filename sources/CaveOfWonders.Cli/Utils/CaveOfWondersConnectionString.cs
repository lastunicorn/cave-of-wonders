using System.Text.RegularExpressions;

namespace DustInTheWind.CaveOfWonders.Cli.Utils;

internal partial class CaveOfWondersConnectionString
{
	public string Value { get; }

	public CaveOfWondersConnectionString(string connectionString)
	{
		if (connectionString is null)
			throw new ArgumentNullException(nameof(connectionString));

		Value = PlaceholderPattern().Replace(connectionString, ReplacePlaceholder);
	}

	private static string ReplacePlaceholder(Match match)
	{
		string placeholderName = match.Groups[1].Value;

		if (!Enum.TryParse(placeholderName, out Environment.SpecialFolder specialFolder))
			throw new InvalidOperationException($"Unknown placeholder '{{{placeholderName}}}' in connection string.");

		return Environment.GetFolderPath(specialFolder);
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator string(CaveOfWondersConnectionString connectionString)
	{
		return connectionString.Value;
	}

	[GeneratedRegex(@"\{(\w+)\}")]
	private static partial Regex PlaceholderPattern();
}