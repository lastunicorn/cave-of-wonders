// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Exchange;

[NamedCommand("exchange", Description = "Displays the exchange rate for the specified date.")]
public class ExchangeCommand : IConsoleCommand<PresentExchangeRateResponse>
{
    private readonly IMediator mediator;

    [AnonymousParameter(Order = 1, DisplayName = "Currency", IsOptional = true, Description = "The currency pair to be displayed. Ex: EUR/RON")]
    public string CurrencyPair { get; set; }

    [NamedParameter("today", ShortName = 't', IsOptional = true, Description = "If this flag is set, the exchange rate for today is displayed.")]
    public bool Today { get; set; }

    [NamedParameter("date", ShortName = 'd', IsOptional = true, Description = "The date for which to display the exchange rate.")]
    public DateTime? Date { get; set; }

    [NamedParameter("start-date", ShortName = 's', IsOptional = true, Description = "Works together with end-date to specify a time interval for which to return exchange rates.")]
    public DateTime? StartDate { get; set; }

    [NamedParameter("end-date", ShortName = 'e', IsOptional = true, Description = "Works together with start-date to specify a time interval for which to return exchange rates.")]
    public DateTime? EndDate { get; set; }

    [NamedParameter("year", ShortName = 'y', IsOptional = true, Description = "Specify the year for which to return exchange rate values. Works together with the optional month.")]
    public uint? Year { get; set; }

    [NamedParameter("month", ShortName = 'm', IsOptional = true, Description = "Specify the month of the year for which to return exchange rate values. If year is not specified, this value is ignored.")]
    public uint? Month { get; set; }

    public ExchangeCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

        return await mediator.Send(request);
    }
}