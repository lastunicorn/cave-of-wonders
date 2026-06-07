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

using CaveOfWonders.WebApi.Presentation.Endpoints.Inflation.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Inflation;

/// <summary>
/// API controller for managing and retrieving inflation data.
/// Provides endpoints for retrieving inflation records and importing inflation data from various sources.
/// </summary>
[Route("inflation")]
[ApiController]
public class InflationController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="InflationController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to send requests to the application layer.</param>
    /// <exception cref="ArgumentNullException">Thrown when mediator is null.</exception>
    public InflationController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Retrieves all inflation records stored in the system.
    /// </summary>
    /// <returns>A collection of inflation records with their corresponding values and dates.</returns>
    /// <response code="200">Returns the inflation records successfully retrieved.</response>
    [HttpGet]
    [ProducesResponseType(typeof(InflationResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<InflationResponseDto>> GetInflationRecords()
    {
        PresentInflationRequest request = new();
        PresentInflationResponse response = await mediator.Send(request);

        InflationResponseDto responseDto = InflationResponseDto.FromApplicationResponse(response);
        return Ok(responseDto);
    }

    /// <summary>
    /// Imports inflation data from a specified source (INS website or file).
    /// </summary>
    /// <param name="importInflationDto">The request containing source details for inflation data import.</param>
    /// <returns>A summary of the import operation including counts of processed records.</returns>
    /// <response code="200">Returns the import operation summary if successful.</response>
    /// <response code="400">If the request is invalid, file path is missing, import source is invalid, 
    /// or there are issues with accessing the INS resources.</response>
    /// <response code="500">If an unexpected error occurs while storing data or processing the request.</response>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportInflationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportInflationResponseDto>> ImportInflation([FromBody] ImportInflationDto importInflationDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ImportInflationRequest request = importInflationDto.ToApplicationRequest();
        ImportInflationResponse response = await mediator.Send(request);

        ImportInflationResponseDto responseDto = ImportInflationResponseDto.FromApplicationResponse(response);
        return Ok(responseDto);
    }
}
