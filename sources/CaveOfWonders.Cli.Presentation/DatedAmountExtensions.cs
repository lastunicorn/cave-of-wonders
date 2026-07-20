using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation;

internal static class DatedAmountExtensions
{
	public static string ToDisplayString(this DatedAmount datedAmount)
	{
		if (datedAmount == null)
			return string.Empty;

		return $"{datedAmount.Value:N2} {datedAmount.Currency}";
	}
}