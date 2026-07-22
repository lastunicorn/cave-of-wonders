using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentWealth;

public class PresentWealthRequest : IRequest<PresentWealthResponse>
{
	public DateOnly? Date { get; set; }

	public string Currency { get; set; }

	public bool IncludeInactive { get; set; }
}