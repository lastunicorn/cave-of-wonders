using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class AmountExtensions
{
	public static string ToDisplayString(this Amount amount)
	{
		if (amount == null)
			return string.Empty;

		return $"{amount.Value:N2} {amount.Currency}";
	}
}