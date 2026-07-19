using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.CreatePot;

public class CreatePotRequest : IRequest<CreatePotResponse>
{
	public string Name { get; set; }

	public string Description { get; set; }

	public DateOnly? StartDate { get; set; }

	public string Currency { get; set; }
}