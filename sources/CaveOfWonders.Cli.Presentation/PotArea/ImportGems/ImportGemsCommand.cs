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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.ImportGems;

[NamedCommand("import-gems", Description = "Imports gems from csv exported files of the sheets of my ods file.")]
internal class ImportGemsCommand : IConsoleCommand<ImportGemsViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("source-file", ShortName = 'f', Description = "The full path of the xlsx file.")]
    public string SourceFilePath { get; set; }

    [NamedParameter("mappings-file", ShortName = 'm', Description = "The full path of the mappings file. This file specify which column from the spreadsheet to be imported in which pot.")]
    public string MappingsFilePath { get; set; }

    [NamedParameter("overwrite", ShortName = 'x', IsOptional = true, Description = "If specified, the entire pot will be cleared and populated with the current imported gems.")]
    public bool Overwrite { get; set; }

    public ImportGemsCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportGemsViewModel> Execute()
    {
        ImportGemsRequest request = new()
        {
            SourceFilePath = SourceFilePath,
            MappingsFilePath = MappingsFilePath,
            Overwrite = Overwrite
        };

        ImportGemsResponse response = await mediator.Send(request);

        return new ImportGemsViewModel
        {
            Report = response.Report,
        };
    }
}