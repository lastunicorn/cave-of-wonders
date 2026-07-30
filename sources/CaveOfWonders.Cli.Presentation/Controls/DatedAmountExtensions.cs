using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;

internal static class DatedAmountExtensions
{
	public static string ToDisplayString(this DatedAmount datedAmount)
	{
		if (datedAmount == null)
			return string.Empty;

		return $"{datedAmount.Value:N2} {datedAmount.Currency}";
	}
}