using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.RequestR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.FxArea.Fx;

[NamedCommand("fx", Description = "Displays the exchange rate for the specified date.")]
public class FxCommand : IConsoleCommand<PresentExchangeRateResponse>
{
	private readonly RequestBus requestBus;

	[AnonymousParameter(Order = 1, DisplayName = "Currency", IsMandatory = false, Description = "The currency pair to be displayed. Ex: EUR/RON")]
	public string CurrencyPair { get; set; }

	[NamedParameter("today", ShortName = 't', IsMandatory = false, Description = "If this flag is set, the exchange rate for today is displayed.")]
	public bool Today { get; set; }

	[NamedParameter("date", ShortName = 'd', IsMandatory = false, Description = "The date for which to display the exchange rate.")]
	public DateOnly? Date { get; set; }

	[NamedParameter("start-date", ShortName = 's', IsMandatory = false, Description = "Works together with end-date to specify a time interval for which to return exchange rates.")]
	public DateOnly? StartDate { get; set; }

	[NamedParameter("end-date", ShortName = 'e', IsMandatory = false, Description = "Works together with start-date to specify a time interval for which to return exchange rates.")]
	public DateOnly? EndDate { get; set; }

	[NamedParameter("year", ShortName = 'y', IsMandatory = false, Description = "Specify the year for which to return exchange rate values. Works together with the optional month.")]
	public uint? Year { get; set; }

	[NamedParameter("month", ShortName = 'm', IsMandatory = false, Description = "Specify the month of the year for which to return exchange rate values. If year is not specified, this value is ignored.")]
	public uint? Month { get; set; }

	public FxCommand(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
	}

	public async Task<PresentExchangeRateResponse> Execute()
	{
		PresentExchangeRateRequest request = new()
		{
			CurrencyPair = CurrencyPair,
			Today = Today,
			Date = Date,
			StartDate = StartDate,
			EndDate = EndDate,
			Year = Year,
			Month = Month
		};

		return await requestBus.SendAsync<PresentExchangeRateRequest, PresentExchangeRateResponse>(request);
	}
}