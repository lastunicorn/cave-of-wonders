using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application;

internal class CurrencyConvertor
{
	private readonly ExchangeRate exchangeRate;
	private readonly bool isDirect;
	private readonly DateOnly date;

	public DateOnly Date => exchangeRate?.Date ?? date;

	public CurrencyConvertor(DateOnly date)
	{
		this.date = date;
	}

	public CurrencyConvertor(ExchangeRate exchangeRate, bool isDirect = true)
	{
		this.exchangeRate = exchangeRate ?? throw new ArgumentNullException(nameof(exchangeRate));
		this.isDirect = isDirect;
	}

	public decimal Convert(decimal value)
	{
		if (exchangeRate == null)
			return 0;

		return isDirect
			? exchangeRate.Convert(value)
			: exchangeRate.ConvertBack(value);
	}
}