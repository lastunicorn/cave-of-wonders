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

using DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.CreatePot;

[NamedCommand("create-pot", Description = "Create a new pot.")]
internal class CreatePotCommand : IConsoleCommand<CreatePotViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("name", ShortName = 'n', Description = "The name of the new pot.")]
    public string Name { get; set; }

    [NamedParameter("description", ShortName = 'd', IsOptional = true, Description = "Optional description for the pot.")]
    public string Description { get; set; }

    [NamedParameter("start-date", ShortName = 's', IsOptional = true, Description = "Optional start date for the pot. Default value = today")]
    public DateTime? StartDate { get; set; }

    [NamedParameter("currency", ShortName = 'c', Description = "The currency for this pot.")]
    public string Currency { get; set; }

    public CreatePotCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<CreatePotViewModel> Execute()
    {
        CreatePotRequest request = new()
        {
            Name = Name,
            Description = Description,
            StartDate = StartDate,
            Currency = Currency
        };

        CreatePotResponse response = await mediator.Send(request);
        
        return new CreatePotViewModel
        {
            PotName = response.Name,
            Description = response.Description,
            StartDate = response.StartDate,
            Currency = response.Currency,
            PotId = response.PotId
        };
    }
}