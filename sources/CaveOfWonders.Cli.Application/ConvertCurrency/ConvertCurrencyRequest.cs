using DustInTheWind.CaveOfWonders.DataTypes;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;

public class ConvertCurrencyRequest
{
	public decimal InitialValue { get; set; }

	public CurrencyPair CurrencyPair { get; set; }

	public DateOnly? Date { get; set; }
}