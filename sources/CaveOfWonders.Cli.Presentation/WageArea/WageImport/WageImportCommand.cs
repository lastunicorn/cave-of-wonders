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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportAverageWage;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.WageArea.WageImport;

[NamedCommand("wage-import", Description = "Imports the average wage data from the INS web page.")]
internal class WageImportCommand : IConsoleCommand<WageImportViewModel>
{
    private readonly IMediator mediator;

    public WageImportCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<WageImportViewModel> Execute()
    {
        WageImportRequest request = new();
        WageImportResponse response =  await mediator.Send(request);
        
        return new WageImportViewModel
        {
            TotalCount = response.TotalCount,
            AddedCount = response.AddedCount,
            UpdatedCount = response.UpdatedCount,
            DeletedCount = response.DeletedCount
        };
    }
}