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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.InflationArea.ImportInflation;

[NamedCommand("import-inflation")]
internal class ImportInflationCommand : IConsoleCommand<ImportInflationViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("source-file", ShortName = 'f', IsOptional = false)]
    public string SourceFile { get; set; }

    public ImportInflationCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportInflationViewModel> Execute()
    {
        ImportInflationRequest request = new()
        {
            SourceFilePath = SourceFile
        };
        ImportInflationResponse response = await mediator.Send(request);

        return new ImportInflationViewModel
        {
            Records = response.Records
        };
    }
}