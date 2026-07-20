using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Infrastructure;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

public class PresentGemsRequest : IRequest<PresentGemsResponse>
{
	public PotFlexId PotId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public DateOnly? Date { get; set; }

	public MonthDate Month { get; set; }

	public bool ExcludeInternal { get; set; }
}