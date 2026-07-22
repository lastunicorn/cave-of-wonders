using DustInTheWind.ConsoleTools;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.Cpi;

internal class ChartLineControl
{
	private const int Threshold = 3;

	public decimal Value { get; set; }

	public void Display()
	{
		if (Value <= 0)
			return;

		int roundedValue = (int)Math.Round(Math.Max(0, Value));

		int safeValue = Math.Min(Threshold, roundedValue);
		DisplayValue(safeValue, ConsoleColor.DarkGray);

		int painfulValue = roundedValue - safeValue;
		if (painfulValue > 0)
			DisplayValue(painfulValue, ConsoleColor.White);
	}

	private static void DisplayValue(int value, ConsoleColor color)
	{
		string chartLine = new('.', value);
		CustomConsole.Write(color, chartLine);
	}
}