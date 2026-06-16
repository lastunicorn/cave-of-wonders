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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Gem;

[NamedCommand("gem", Description = "Display a list of gems filtered by pot.")]
internal class GemCommand : IConsoleCommand<GemCommandViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("pot", Description = "The pot id for which to display the gems.")]
    public string PotId { get; set; }

    [NamedParameter("start-date", IsMandatory = false, Description = "The start date for which to display the gems.")]
    public DateOnly? StartDate { get; set; }

    [NamedParameter("end-date", IsMandatory = false, Description = "The end date for which to display the gems.")]
    public DateOnly? EndDate { get; set; }

    [NamedParameter("date", IsMandatory = false, Description = "The date for which to display the gems.")]
    public DateOnly? Date { get; set; }

    [NamedParameter("month", IsMandatory = false, Description = "The month for which to display the gems.")]
    public string Month { get; set; }

    public GemCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<GemCommandViewModel> Execute()
    {
        PresentGemsRequest request = new()
        {
            PotId = PotId,
            Date = Date
        };
        
        PresentGemsResponse response = await mediator.Send(request);

        return new GemCommandViewModel
        {
            Gems = response.Gems
        };
    }
}