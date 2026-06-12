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

using System.Globalization;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.PotOverview;

[NamedCommand("pot-overview", Description = "Display the state of the pots for a specific date.")]
internal class PotOverviewCommand : IConsoleCommand<PotOverviewViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("date", ShortName = 'd', IsMandatory = false, Description = "The date for which to display the state of the cave. Default value = today")]
    public DateOnly? Date { get; set; }

    [NamedParameter("currency", ShortName = 'c', IsMandatory = false)]
    public string Currency { get; set; }

    [NamedParameter("all", ShortName = 'a', IsMandatory = false, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    [NamedParameter("culture", ShortName = 'u', IsMandatory = false, Description = "The culture info used for displaying the data.")]
    public CultureInfo Culture { get; set; }

    public PotOverviewCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<PotOverviewViewModel> Execute()
    {
        PresentPotsRequest request = new()
        {
            Date = Date,
            Currency = Currency,
            IncludeInactive = IncludeInactivePots
        };

        PresentPotsResponse response = await mediator.Send(request);

        return new PotOverviewViewModel(response)
        {
            Culture = Culture,
        };
    }
}