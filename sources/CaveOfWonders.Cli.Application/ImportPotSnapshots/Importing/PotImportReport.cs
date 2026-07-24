namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

public class PotImportReport
{
	public string PotName { get; init; }

	public Guid PotId { get; init; }

	public int SkipExistsCount { get; set; }

	public int SkipNotActiveCount { get; set; }

	public int AddCount { get; set; }
}