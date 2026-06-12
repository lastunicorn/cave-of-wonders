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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.CpiArea.CpiImport;

[NamedCommand("cpi-import", Description = "Imports Romanian annual inflation values from the INS web page or a file.")]
internal class CpiImportCommand : IConsoleCommand<CpiImportViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("source-type", ShortName = 't', IsMandatory = false, Description = "The source of the imported data. (file - text file; web - html webpage from https://insse.ro)")]
    public CpiImportSourceType ImportSourceType { get; set; } = CpiImportSourceType.Web;

    [NamedParameter("source-file", ShortName = 'f', IsMandatory = false)]
    public string SourceFile { get; set; }

    public CpiImportCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<CpiImportViewModel> Execute()
    {
        ImportCpiRequest request = new()
        {
            ImportSource = ImportSourceType switch
            {
                CpiImportSourceType.File => ImportSource.File,
                CpiImportSourceType.Web => ImportSource.Web,
                _ => throw new ArgumentOutOfRangeException()
            },
            SourceFilePath = SourceFile
        };
        ImportCpiResponse response = await mediator.Send(request);

        return new CpiImportViewModel
        {
            AddedCount = response.AddedCount,
            UpdatedCount = response.UpdatedCount,
            TotalCount = response.TotalCount
        };
    }
}