using DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;
using DustInTheWind.CaveOfWonders.Cli.Presentation.Controls;
using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.FxConvert;

[NamedCommand("fx-convert", Description = "Convert a value from one currency into another.")]
internal class FxConvertCommand : IConsoleCommand<FxConvertViewModel>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(Order = 1, DisplayName = "value", IsMandatory = true, Description = "The value to be converted.")]
	public decimal InitialValue { get; set; }

	[AnonymousParameter(Order = 2, DisplayName = "source currency", IsMandatory = true, Description = "The currency of the value to be converted.")]
	public string SourceCurrencyId { get; set; }

	[AnonymousParameter(Order = 3, DisplayName = "destination currency", IsMandatory = true, Description = "The destination currency.")]
	public string DestinationCurrencyId { get; set; }

	[NamedParameter("date", ShortName = 'd', IsMandatory = false, Description = "The date of the exchange rate to use for conversion.")]
	public DateOnly? Date { get; set; }

	public FxConvertCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<FxConvertViewModel> Execute()
	{
		ConvertCurrencyRequest request = new()
		{
			InitialValue = InitialValue,
			CurrencyPair = new CurrencyPair
			{
				Currency1 = SourceCurrencyId,
				Currency2 = DestinationCurrencyId
			},
			Date = Date
		};

		ConvertCurrencyResponse response = await requestBus.SendAsync<ConvertCurrencyRequest, ConvertCurrencyResponse>(request);

		return new FxConvertViewModel
		{
			InitialValue = response.InitialValue,
			ConvertedValue = response.ConvertedValue,
			SourceCurrency = response.ExchangeRate.SourceCurrency,
			DestinationCurrency = response.ExchangeRate.DestinationCurrency,
			ExchangeRate = new ExchangeRateViewModel(response.ExchangeRate, response.IsDateCurrent)
		};
	}
}