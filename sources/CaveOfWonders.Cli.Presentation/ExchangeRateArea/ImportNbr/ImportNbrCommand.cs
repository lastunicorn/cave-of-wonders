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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportFromNbrFile;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.ExchangeRateArea.ImportNbr;

[NamedCommand("import-nbr", Description = "Import exchange rates from a local nbr file downloaded from BNR website.")]
internal class ImportNbrCommand : IConsoleCommand<ImportFromNbrResponse>
{
    private readonly IMediator mediator;

    [NamedParameter("source-file", ShortName = 'f')]
    public string SourceFilePath { get; set; }

    public ImportNbrCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportFromNbrResponse> Execute()
    {
        ImportFromNbrRequest request = new()
        {
            SourceFilePath = SourceFilePath
        };
        ImportFromNbrResponse response = await mediator.Send(request);

        return response;
    }
}