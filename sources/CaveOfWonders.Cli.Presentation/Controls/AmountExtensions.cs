using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

internal static class AmountExtensions
{
	public static string ToDisplayString(this Amount amount)
	{
		if (amount == null)
			return string.Empty;

		return $"{amount.Value:N2} {amount.Currency}";
	}
}