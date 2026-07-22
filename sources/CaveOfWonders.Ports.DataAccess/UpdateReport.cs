namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public class UpdateReport
{
	public DateOnly Date { get; set; }

	public string CurrencyPair { get; set; }

	public decimal OldValue { get; set; }

	public decimal NewValue { get; set; }
}