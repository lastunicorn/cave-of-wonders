using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentGems;

public class PresentGemsRequest
{
	public PotFlexId PotId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? EndDate { get; set; }

	public DateOnly? Date { get; set; }

	public MonthAndYear Month { get; set; }

	public int? Year { get; set; }

	public bool CurrentMonth { get; set; }

	public bool LastMonth { get; set; }

	public bool LatestMonth { get; set; }

	public bool CurrentYear { get; set; }

	public bool LastYear { get; set; }

	public bool LatestYear { get; set; }

	public bool ExcludeInternal { get; set; }
}