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

using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.GemImport;

[NamedCommand("gem-import", Description = "Import gems.")]
internal class GemImportCommand : IConsoleCommand<GemImportViewModel>
{
    private readonly IMediator mediator;

    public GemImportCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<GemImportViewModel> Execute()
    {
        ImportGemsRequest request = new()
        {
            FilePath = @"/nfs/YubabaAlez/finanțe/mintos/account statements/2026/05 - mai/2026 05 - account-statement.csv"
        };
        
        ImportGemsResponse response = await mediator.Send(request);
        
        return new GemImportViewModel
        {
            UpdatedGemsCount = response.UpdatedGemsCount,
            AddedGemsCount = response.AddedGemsCount,
            TotalGemsCount = response.TotalGemsCount
        };
    }
}