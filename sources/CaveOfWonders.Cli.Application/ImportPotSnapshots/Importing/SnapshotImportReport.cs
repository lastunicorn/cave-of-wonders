using System.Collections;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

public class SnapshotImportReport : IEnumerable<PotImportReport>
{
	private readonly List<PotImportReport> potImportReports = [];

	public PotImportReport GetOrCreateReport(Pot pot)
	{
		PotImportReport potImportReport = potImportReports.FirstOrDefault(x => x.PotId == pot.Id);

		return potImportReport ?? CreateAndAddNewReportFor(pot);
	}

	private PotImportReport CreateAndAddNewReportFor(Pot pot)
	{
		PotImportReport potImportReport = new()
		{
			PotId = pot.Id,
			PotName = pot.Name
		};

		potImportReports.Add(potImportReport);

		return potImportReport;
	}

	public PotImportReport GetOrCreateReport(Guid potId)
	{
		PotImportReport potImportReport = potImportReports.FirstOrDefault(x => x.PotId == potId);

		return potImportReport ?? CreateAndAddNewReportFor(potId);
	}

	private PotImportReport CreateAndAddNewReportFor(Guid potId)
	{
		PotImportReport potImportReport = new()
		{
			PotId = potId
		};

		potImportReports.Add(potImportReport);

		return potImportReport;
	}

	public IEnumerator<PotImportReport> GetEnumerator()
	{
		return potImportReports.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}