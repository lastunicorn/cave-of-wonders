namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxImport;

internal static class NumberExtensions
{
	public static string ToStringOrEmpty(this int number, string emptyString = "")
	{
		return number == 0
			? emptyString
			: number.ToString();
	}
}