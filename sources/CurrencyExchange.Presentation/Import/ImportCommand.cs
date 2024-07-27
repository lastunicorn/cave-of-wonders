// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
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
using DustInTheWind.CurrencyExchange.Application.ImportFromNbrWebsite;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Presentation.Import;

[NamedCommand("import", Description = "Import exchange rates directly from BNR website, from nbr files.")]
internal class ImportCommand : IConsoleCommand<ImportFromNbrWebsiteResponse>
{
    private readonly IMediator mediator;

    [NamedParameter("year", ShortName = 'y')]
    public int Year { get; set; }

    public ImportCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportFromNbrWebsiteResponse> Execute()
    {
        ImportFromNbrWebsiteRequest request = new()
        {
            Year = Year
        };
        ImportFromNbrWebsiteResponse response = await mediator.Send(request);

        return response;
    }
}