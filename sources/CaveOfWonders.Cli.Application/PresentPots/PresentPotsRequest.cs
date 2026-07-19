using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

public class PresentPotsRequest : IRequest<PresentPotsResponse>
{
	public DateOnly? Date { get; set; }

	public string Currency { get; set; }

	public bool IncludeInactive { get; set; }
}