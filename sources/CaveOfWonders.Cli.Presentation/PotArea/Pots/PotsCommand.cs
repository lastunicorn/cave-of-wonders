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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

[NamedCommand("pots", Description = "Display the current value of the pots.")]
internal class PotsCommand : IConsoleCommand<PresentPotsViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("all", ShortName = 'a', IsOptional = true, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    public PotsCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<PresentPotsViewModel> Execute()
    {
        PresentPotsRequest request = new()
        {
            IncludeInactivePots = IncludeInactivePots
        };

        PresentPotsResponse response = await mediator.Send(request);

        return new PresentPotsViewModel
        {
            Date = response.Date,
            Pots = response.Pots
                .Select(x => new PotInfoViewModel(x))
                .ToList()
        };
    }
}