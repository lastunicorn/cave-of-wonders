using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Infrastructure;

namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public class GemFilter
{
	public Guid? PotId { get; set; }

	public DateOnly? Date { get; set; }

	public MonthDate? Month { get; set; }

	public List<GemCategory> IncludeCategories { get; set; }
	
	//public List<GemCategory> ExcludeCategories { get; set; }

	public string ExternalId { get; set; }
}