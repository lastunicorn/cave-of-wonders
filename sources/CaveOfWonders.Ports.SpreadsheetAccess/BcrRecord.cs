namespace DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

public class BcrRecord
{
	public DateTime Date { get; set; }

	public decimal TotalLei { get; set; }

	public decimal? CurrentAccountLei { get; set; }

	public decimal? SavingsAccountLei { get; set; }

	public decimal? DepositAccountLei { get; set; }

	public decimal? CurrentAccountEuro { get; set; }

	public decimal? CurrentAccountEuroConvertedInLei { get; set; }
}