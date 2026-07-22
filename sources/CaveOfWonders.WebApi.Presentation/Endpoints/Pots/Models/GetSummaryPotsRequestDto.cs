using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

public class GetSummaryPotsRequestDto
{
	[FromQuery]
	public DateOnly? Date { get; set; }

	[FromQuery]
	public string Currency { get; set; }

	[FromQuery]
	public bool IncludeInactive { get; set; }
}