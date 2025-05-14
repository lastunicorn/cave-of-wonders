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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using DustInTheWind.ConsoleTools.Commando;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.InflationArea.Inflation;

[NamedCommand("inflation", Description = "Display the inflation rates.")]
internal class InflationCommand : IConsoleCommand<InflationViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("output", ShortName = 'o', IsOptional = true, Description = "Path to the output file.")]
    public string OutputPath { get; set; }

    public InflationCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<InflationViewModel> Execute()
    {
        PresentInflationRequest request = new()
        {
            OutputPath = OutputPath
        };
        PresentInflationResponse response = await mediator.Send(request);

        return new InflationViewModel
        {
            Records = response.InflationRecords
        };
    }
}