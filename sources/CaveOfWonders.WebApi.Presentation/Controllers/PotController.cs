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

using CaveOfWonders.WebApi.Presentation.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Controllers;

[Route("pot")]
public class PotController : ControllerBase
{
    private readonly IMediator mediator;

    public PotController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets a list with all pots with their values.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PresentPotsResponse>> GetPots(GetPotsRequestDto getPotsRequestDto)
    {
        PresentPotsRequest request = new()
        {
            Date = getPotsRequestDto.Date,
            Currency = getPotsRequestDto.Currency,
            IncludeInactive = getPotsRequestDto.IncludeInactive
        };

        PresentPotsResponse response = await mediator.Send(request);
        return Ok(response);
    }

    /// <summary>
    /// Gets a specific pot by its identifier.
    /// </summary>
    [HttpGet("{potIdentifier}")]
    public async Task<ActionResult<PresentPotResponse>> GetPot(GetPotRequestDto getPotRequestDto)
    {
        try
        {
            PresentPotRequest request = getPotRequestDto.ToApplicationRequest();
            PresentPotResponse response = await mediator.Send(request);

            GetPotResponseDto responseDto = GetPotResponseDto.FromApplicationResponse(response);
            return Ok(responseDto);
        }
        catch (PotIdentifierNotSpecifiedException)
        {
            return BadRequest("Pot identifier is required");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
