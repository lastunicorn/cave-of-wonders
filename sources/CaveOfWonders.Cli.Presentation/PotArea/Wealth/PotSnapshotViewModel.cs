using DustInTheWind.CaveOfWonders.Cli.Application;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Wealth;

internal class PotSnapshotViewModel
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public DatedAmount OriginalValue { get; set; }

	public bool IsValueActual { get; set; }

	public bool IsValueAlreadyNormal { get; set; }

	public DateOnly? Date { get; set; }

	public DatedAmount NormalizedValue { get; set; }

	public bool IsNormalizedCurrent { get; set; }

	public bool IsPotActive { get; set; }
}