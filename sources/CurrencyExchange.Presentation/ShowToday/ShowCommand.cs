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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.CurrencyExchange.Application.PresentToday;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Presentation.ShowToday;

[NamedCommand("show")]
public class ShowCommand : IConsoleCommand<PresentTodayResponse>
{
    private readonly IMediator mediator;

    [NamedParameter("today", ShortName = 't', IsOptional = true, Description = "If this flag is set, the exchange rate for today is displayed.")]
    public bool Today { get; set; }

    [NamedParameter("date", ShortName = 'd', IsOptional = true, Description = "The date for which to display the exchange rate.")]
    public List<string> Date { get; set; }

    [NamedParameter("currency", ShortName = 'c', IsOptional = true, Description = "The currency pair to be displayed. Ex: EUR/RON")]
    public string CurrencyPair { get; set; }

    public ShowCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<PresentTodayResponse> Execute()
    {
        PresentTodayRequest request = new()
        {
            Dates = Date?.Select(DateTime.Parse).ToList(),
            Today = Today,
            CurrencyPair = CurrencyPair
        };
        PresentTodayResponse response = await mediator.Send(request);

        return response;
    }
}