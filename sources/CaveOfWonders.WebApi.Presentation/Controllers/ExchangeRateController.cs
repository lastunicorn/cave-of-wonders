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
using DustInTheWind.CaveOfWonders.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Controllers;

[Route("exchange-rate")]
public class ExchangeRateController : ControllerBase
{
    private readonly IMediator mediator;

    public ExchangeRateController(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get exchange rates
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PresentExchangeRateResponse>> GetExchangeRates([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string currency1 = null, [FromQuery] string currency2 = null)
    {
        PresentExchangeRateRequest request = new()
        {
            StartDate = startDate,
            EndDate = endDate
        };

        if (currency1 != null && currency2 != null)
        {
            CurrencyPairDto currencyPairDto = new()
            {
                Currency1 = currency1,
                Currency2 = currency2
            };

            request.CurrencyPair = currencyPairDto.ToDomain();
        }

        try
        {
            PresentExchangeRateResponse response = await mediator.Send(request);
            return Ok(response);
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
    /// Convert a value from one currency to another
    /// </summary>
    [HttpGet("convert")]
    public async Task<ActionResult<ConvertResponse>> Convert(ExchangeRateConvertRequestDto exchangeRateConvertRequestDto)
    {
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
    /// Import exchange rates
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<ImportExchangeRatesResponse>> ImportExchangeRates([FromBody] ImportExchangeRatesRequest request)
    {
        try
        {
            ImportExchangeRatesResponse response = await mediator.Send(request);
            return Ok(response);
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
