namespace DustInTheWind.CaveOfWonders.Ports.DataAccess;

public class ExchangeRateImportReport
{
	public int TotalCount { get; set; }

	public int AddedCount { get; set; }

	public int ExistingUpdatedCount { get; set; }

	public int ExistingIdenticalCount { get; set; }

	public int NewDuplicateIdenticalCount { get; set; }

	public int NewDuplicateDifferentCount { get; set; }

	public List<UpdateReport> Updates { get; } = new();

	public List<DuplicateReport> Duplicates { get; } = new();
}