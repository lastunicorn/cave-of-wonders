using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

public class FindPotRequestDto
{
	[FromQuery(Name = "potIdentifier")]
	public string PotIdentifier { get; set; }

	[FromQuery]
	public bool IncludeInactive { get; set; }

	internal PresentPotRequest ToApplicationRequest()
	{
		return new PresentPotRequest
		{
			PotFlexId = PotIdentifier,
			IncludeInactivePots = IncludeInactive
		};
	}
}