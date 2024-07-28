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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.Pot;

[NamedCommand("pot", Description = "Display details about a specific pot.")]
internal class PotCommand : IConsoleCommand<PotCommandViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("name", ShortName = 'n', IsOptional = true, Description = "The name of the pot for which to display details.")]
    public string PotName { get; set; }

    [NamedParameter("Id", ShortName = 'i', IsOptional = true, Description = "The Id of the pot for which to display details. Partial Id (ex: first 8 characters) is allowed to be provided.")]
    public string PotId { get; set; }

    [AnonymousParameter(DisplayName = "Pot Identifier", Order = 1, IsOptional = true, Description = "Name or id of the pot. Partial id is accepted.")]
    public string PotIdentifier { get; set; }

    [NamedParameter("all", ShortName = 'a', IsOptional = true, Description = "Display all pots, including the inactive ones. Default = false.")]
    public bool IncludeInactivePots { get; set; }

    public PotCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<PotCommandViewModel> Execute()
    {
        PresentPotRequest request = new()
        {
            PotName = PotName,
            PotId = PotId,
            PotIdentifier = PotIdentifier,
            IncludeInactivePots = IncludeInactivePots
        };

        PresentPotResponse response = await mediator.Send(request);

        return new PotCommandViewModel
        {
            PotDetailsViewModels = response.Pots
                .Select(x => new PotDetailsViewModel(x))
                .ToList()
        };
    }
}