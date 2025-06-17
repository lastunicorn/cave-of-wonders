// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.Convert;

[NamedCommand("convert", Description = "Convert a value from one currency into another.")]
internal class ConvertCommand : IConsoleCommand<ConvertViewModel>
{
    private readonly IMediator mediator;

    [AnonymousParameter(Order = 1, DisplayName = "value", IsOptional = false, Description = "The value to be converted.")]
    public decimal InitialValue { get; set; }

    [AnonymousParameter(Order = 2, DisplayName = "source currency", IsOptional = false, Description = "The currency of the value to be converted.")]
    public string SourceCurrencyId { get; set; }

    [AnonymousParameter(Order = 3, DisplayName = "destination currency", IsOptional = false, Description = "The destination currency.")]
    public string DestinationCurrencyId { get; set; }

    [NamedParameter("date", ShortName = 'd', IsOptional = true, Description = "The date of the exchange rate to use for conversion.")]
    public DateTime? Date { get; set; }

    public ConvertCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ConvertViewModel> Execute()
    {
        ConvertRequest request = new()
        {
            InitialValue = InitialValue,
            SourceCurrency = SourceCurrencyId,
            DestinationCurrency = DestinationCurrencyId,
            Date = Date
        };

        ConvertResponse response = await mediator.Send(request);

        return new ConvertViewModel
        {
            InitialValue = response.InitialValue,
            ConvertedValue = response.ConvertedValue,
            SourceCurrency = response.SourceCurrency,
            DestinationCurrency = response.DestinationCurrency,
            ExchangeRate = new ExchangeRateViewModel(response.ExchangeRate, response.IsDateCurrent)
        };
    }
}