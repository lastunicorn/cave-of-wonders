using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

public class FindPotResponseDto
{
	public List<PotDetailsApiDto> Pots { get; set; }

	internal static FindPotResponseDto From(PresentPotResponse response)
	{
		return new FindPotResponseDto
		{
			Pots = response.PotDetails
				.Select(x => PotDetailsApiDto.From(x))
				.ToList()
		};
	}
}