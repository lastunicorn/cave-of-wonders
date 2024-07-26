// Cave of Wonders
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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.State;

[NamedCommand("state", Description = "Displays the values stored in the cave for a specific date.")]
public class StateCommand : IConsoleCommand<StateViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("date", ShortName = 'd', IsOptional = true, Description = "The date for which to display the state of the cave. Default value = today")]
    public DateTime? Date { get; set; }

    [NamedParameter("currency", ShortName = 'c', IsOptional = true)]
    public string Currency { get; set; }

    [NamedParameter("all", ShortName = 'a', IsOptional = true, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    public StateCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<StateViewModel> Execute()
    {
        PresentStateRequest request = new()
        {
            Date = Date,
            Currency = Currency,
            IncludeInactive = IncludeInactivePots
        };

        PresentStateResponse response = await mediator.Send(request);

        return new StateViewModel(response);
    }
}