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

using CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate;

/// <summary>
/// API controller for managing and retrieving exchange rates between currencies.
/// Provides endpoints for retrieving, converting, and importing exchange rates.
/// </summary>
[Route("exchange-rate")]
[ApiController]
public class ExchangeRateController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExchangeRateController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used to send requests to the application layer.</param>
    /// <exception cref="ArgumentNullException">Thrown when mediator is null.</exception>
    public ExchangeRateController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Retrieves exchange rates based on specified criteria such as date range, specific date, or currency pair.
    /// </summary>
    /// <param name="requestDto">The request containing filter criteria for exchange rates.</param>
    /// <returns>A collection of exchange rates matching the specified criteria.</returns>
    /// <response code="200">Returns the exchange rates successfully retrieved.</response>
    /// <response code="400">If the request is invalid or currency pair is missing.</response>
    /// <response code="404">If no exchange rates are found for the specified criteria.</response>
    /// <response code="500">If an unexpected error occurs while processing the request.</response>
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
            PresentExchangeRateRequest request = requestDto.ToApplicationRequest();
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
    /// Converts a monetary value from one currency to another using the latest or specified date exchange rate.
    /// </summary>
    /// <param name="exchangeRateConvertRequestDto">The request containing the value to convert, source and destination currencies, and optional date.</param>
    /// <returns>The converted value and details about the exchange rate used.</returns>
    /// <response code="200">Returns the conversion result successfully calculated.</response>
    /// <response code="400">If the request is invalid or the exchange rate is unusable.</response>
    /// <response code="500">If an unexpected error occurs while processing the request.</response>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ExchangeRateConvertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExchangeRateConvertResponseDto>> Convert([FromQuery] ExchangeRateConvertRequestDto exchangeRateConvertRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ConvertRequest request = exchangeRateConvertRequestDto.ToApplication();
        ConvertResponse response = await mediator.Send(request);

        ExchangeRateConvertResponseDto responseDto = ExchangeRateConvertResponseDto.FromApplicationResponse(response);

        return Ok(responseDto);
    }

    /// <summary>
    /// Imports exchange rates from various sources such as the BNR website or files.
    /// </summary>
    /// <param name="requestDto">The request containing import source details and optional parameters.</param>
    /// <returns>A summary of the import operation including counts of added, updated, and duplicate records.</returns>
    /// <response code="200">Returns the import operation summary if successful.</response>
    /// <response code="400">If the request is invalid, BNR website access fails, or import file access fails.</response>
    /// <response code="500">If an unexpected error occurs while processing the request.</response>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportExchangeRatesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportExchangeRatesResponseDto>> ImportExchangeRates([FromBody] ImportExchangeRatesRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ImportExchangeRatesRequest request = requestDto.ToApplicationRequest();
        ImportExchangeRatesResponse response = await mediator.Send(request);

        ImportExchangeRatesResponseDto responseDto = ImportExchangeRatesResponseDto.FromApplication(response);
        return Ok(responseDto);
    }
}
