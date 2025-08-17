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

using CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots;

/// <summary>
/// API controller for managing and retrieving financial pots (money containers).
/// Provides endpoints for retrieving all pots and specific pot details.
/// </summary>
[Route("pots")]
[ApiController]
public class PotsController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PotsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to send requests to the application layer.</param>
    /// <exception cref="ArgumentNullException">Thrown when mediator is null.</exception>
    public PotsController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Retrieves a list of all financial pots with their current values.
    /// </summary>
    /// <param name="getPotsRequestDto">Request parameters including optional date for historical values, 
    /// currency for value conversion, and flag to include inactive pots.</param>
    /// <returns>A collection of pots with their values and metadata.</returns>
    /// <response code="200">Returns the list of pots successfully retrieved.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(GetSummaryPotsResponseDto), StatusCodes.Status200OK)]
    public async Task<GetSummaryPotsResponseDto> GetSummaryPots(GetSummaryPotsRequestDto getPotsRequestDto)
    {
        PresentPotsRequest request = new()
        {
            Date = getPotsRequestDto.Date,
            Currency = getPotsRequestDto.Currency,
            IncludeInactive = getPotsRequestDto.IncludeInactive
        };

        PresentPotsResponse response = await mediator.Send(request);

        return GetSummaryPotsResponseDto.From(response);
    }

    /// <summary>
    /// Retrieves detailed information about all pots matching the specified identifier.
    /// </summary>
    /// <param name="getPotRequestDto">Request parameters including the pot identifier, 
    /// optional date for historical values, and currency for value conversion.</param>
    /// <returns>Detailed information about the requested pot.</returns>
    /// <response code="200">Returns the pot details successfully retrieved.</response>
    /// <response code="400">If the pot identifier is not specified.</response>
    /// <response code="500">If an unexpected error occurs while processing the request.</response>
    [ProducesResponseType(typeof(FindPotResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<FindPotResponseDto> FindPot(FindPotRequestDto getPotRequestDto)
    {
        PresentPotRequest request = new()
        {
            PotIdentifier = getPotRequestDto.PotIdentifier,
            IncludeInactivePots = getPotRequestDto.IncludeInactive
        };
        PresentPotResponse response = await mediator.Send(request);

        return FindPotResponseDto.From(response);
    }

    /// <summary>
    /// Retrieves detailed information about a specific financial pot by its identifier.
    /// </summary>
    /// <param name="potIdentifier">The pot identifier.</param>
    /// <returns>Detailed information about the requested pot.</returns>
    /// <response code="200">Returns the pot details successfully retrieved.</response>
    /// <response code="400">If the pot identifier is not specified.</response>
    /// <response code="500">If an unexpected error occurs while processing the request.</response>
    [HttpGet("{potIdentifier}")]
    [ProducesResponseType(typeof(PotDetailsApiDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<PotDetailsApiDto> GetPot(string potIdentifier)
    {
        PresentPotRequest request = new()
        {
            PotIdentifier = potIdentifier
        };
        PresentPotResponse response = await mediator.Send(request);

        return PotDetailsApiDto.From(response.Pots.FirstOrDefault());
    }
}
