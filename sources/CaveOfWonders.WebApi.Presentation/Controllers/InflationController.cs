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
using DustInTheWind.CaveOfWonders.Cli.Application.ImportInflation;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentInflation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Controllers;

[Route("inflation")]
[ApiController]
public class InflationController : ControllerBase
{
    private readonly IMediator mediator;

    public InflationController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get inflation records
    /// </summary>
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
    /// Import inflation data
    /// </summary>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportInflationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportInflationResponseDto>> ImportInflation([FromBody] ImportInflationDto importInflationDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ImportInflationRequest request = importInflationDto.ToApplicationRequest();
            ImportInflationResponse response = await mediator.Send(request);

            ImportInflationResponseDto responseDto = ImportInflationResponseDto.FromApplicationResponse(response);
            return Ok(responseDto);
        }
        catch (InflationFileNotProvidedException)
        {
            return BadRequest("Inflation file path not provided");
        }
        catch (InvalidImportSourceException ex)
        {
            return BadRequest($"Invalid import source: {ex.Message}");
        }
        catch (InsWebPageException ex)
        {
            return BadRequest($"Failed to access INS web page: {ex.Message}");
        }
        catch (InsFileException ex)
        {
            return BadRequest($"Failed to process INS file: {ex.Message}");
        }
        catch (DataStorageException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to store data: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}