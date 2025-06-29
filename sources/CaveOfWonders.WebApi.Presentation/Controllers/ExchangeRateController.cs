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
using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Controllers;

[Route("exchange-rate")]
[ApiController]
public class ExchangeRateController : ControllerBase
{
    private readonly IMediator mediator;

    public ExchangeRateController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get exchange rates based on specified criteria.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ExchangeRateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateResponseDto>> GetExchangeRates([FromQuery] ExchangeRateRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            PresentExchangeRateRequest request = requestDto.ToApplication();
            PresentExchangeRateResponse response = await mediator.Send(request);

            ExchangeRateResponseDto responseDto = ExchangeRateResponseDto.FromApplication(response);
            return Ok(responseDto);
        }
        catch (ExchangeRateNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CurrencyPairMissingException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Convert a value from one currency to another.
    /// </summary>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ExchangeRateConvertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateConvertResponseDto>> Convert([FromQuery] ExchangeRateConvertRequestDto exchangeRateConvertRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ConvertRequest request = exchangeRateConvertRequestDto.ToApplication();
            ConvertResponse response = await mediator.Send(request);

            ExchangeRateConvertResponseDto responseDto = ExchangeRateConvertResponseDto.FromApplication(response);

            return Ok(responseDto);
        }
        catch (ExchangeRateUnusableException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Import exchange rates from various sources.
    /// </summary>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportExchangeRatesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportExchangeRatesResponseDto>> ImportExchangeRates([FromBody] ImportExchangeRatesRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            ImportExchangeRatesRequest request = requestDto.ToApplication();
            ImportExchangeRatesResponse response = await mediator.Send(request);

            ImportExchangeRatesResponseDto responseDto = ImportExchangeRatesResponseDto.FromApplication(response);
            return Ok(responseDto);
        }
        catch (BnrWebsiteAccessException ex)
        {
            return BadRequest($"Failed to access BNR website: {ex.Message}");
        }
        catch (ImportFileAccessException ex)
        {
            return BadRequest($"Failed to access import file: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }
}
