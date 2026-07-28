using CaveOfWonders.WebApi.Presentation.Endpoints.Inflation.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentCpi;
using DustInTheWind.RequestR;
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
	private readonly RequestBus requestBus;

	/// <summary>
	/// Initializes a new instance of the <see cref="InflationController"/> class.
	/// </summary>
	/// <param name="requestBus">The request bus used to send requests to the application layer.</param>
	/// <exception cref="ArgumentNullException">Thrown when requestBus is null.</exception>
	public InflationController(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
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
		PresentCpiRequest request = new();
		PresentCpiResponse response = await requestBus.SendAsync<PresentCpiRequest, PresentCpiResponse>(request);

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

		ImportCpiRequest request = importInflationDto.ToApplicationRequest();
		ImportCpiResponse response = await requestBus.SendAsync<ImportCpiRequest, ImportCpiResponse>(request);

		ImportInflationResponseDto responseDto = ImportInflationResponseDto.FromApplicationResponse(response);
		return Ok(responseDto);
	}
}