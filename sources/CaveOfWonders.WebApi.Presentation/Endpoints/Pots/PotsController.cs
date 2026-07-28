using CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;
using DustInTheWind.RequestR;
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
	private readonly RequestBus requestBus;

	/// <summary>
	/// Initializes a new instance of the <see cref="PotsController"/> class.
	/// </summary>
	/// <param name="requestBus">The request bus used to send requests to the application layer.</param>
	/// <exception cref="ArgumentNullException">Thrown when requestBus is null.</exception>
	public PotsController(RequestBus requestBus)
	{
		this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
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
		PresentWealthRequest request = new()
		{
			Date = getPotsRequestDto.Date,
			Currency = getPotsRequestDto.Currency,
			IncludeInactive = getPotsRequestDto.IncludeInactive
		};

		PresentWealthResponse response = await requestBus.SendAsync<PresentWealthRequest, PresentWealthResponse>(request);

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
			PotFlexId = getPotRequestDto.PotIdentifier,
			IncludeInactivePots = getPotRequestDto.IncludeInactive
		};
		PresentPotResponse response = await requestBus.SendAsync<PresentPotRequest, PresentPotResponse>(request);

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
			PotFlexId = potIdentifier
		};
		PresentPotResponse response = await requestBus.SendAsync<PresentPotRequest, PresentPotResponse>(request);

		return PotDetailsApiDto.From(response.PotDetails.FirstOrDefault());
	}
}