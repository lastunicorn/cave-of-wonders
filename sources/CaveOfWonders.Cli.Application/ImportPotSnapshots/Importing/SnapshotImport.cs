using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

internal class SnapshotImport
{
	private readonly ILog log;
	private readonly IUnitOfWork unitOfWork;

	public SnapshotImportReport Report { get; private set; }

	public SnapshotImport(ILog log, IUnitOfWork unitOfWork)
	{
		this.log = log ?? throw new ArgumentNullException(nameof(log));
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task Execute(IEnumerable<SheetValue> sheetValues)
	{
		Report = new SnapshotImportReport();

		foreach (SheetValue sheetValue in sheetValues)
		{
			PotSnapshot potSnapshot = new()
			{
				Date = sheetValue.Date,
				Value = sheetValue.Value
			};

			SnapshotAddReport snapshotAddReport = await AddSnapshot(sheetValue.Key, potSnapshot);
			ProcessSnapshotAddReport(snapshotAddReport);
		}

		LogReports();
	}

	private async Task<SnapshotAddReport> AddSnapshot(Guid key, PotSnapshot potSnapshot)
	{
		ArgumentNullException.ThrowIfNull(potSnapshot);

		Pot pot = await unitOfWork.PotRepository.GetAsync(key);

		if (pot == null)
		{
			return new SnapshotAddReport
			{
				AddStatus = SnapshotAddStatus.PotNotFound,
				Key = key,
				PotSnapshot = potSnapshot
			};
		}

		if (!pot.IsActive(potSnapshot.Date))
		{
			return new SnapshotAddReport
			{
				AddStatus = SnapshotAddStatus.PotNotActive,
				Key = key,
				Pot = pot,
				PotSnapshot = potSnapshot
			};
		}

		HashSet<PotSnapshot> knownSnapshots = await unitOfWork.PotSnapshotRepository.GetByPotIdAsync(key, potSnapshot.Date)
			.ToHashSetAsync();

		if (knownSnapshots.Contains(potSnapshot))
		{
			return new SnapshotAddReport
			{
				AddStatus = SnapshotAddStatus.SnapshotAlreadyExists,
				Key = key,
				Pot = pot,
				PotSnapshot = potSnapshot
			};
		}

		potSnapshot.Pot = pot;
		unitOfWork.PotSnapshotRepository.Add(potSnapshot);

		return new SnapshotAddReport
		{
			AddStatus = SnapshotAddStatus.Success,
			Key = key,
			Pot = pot,
			PotSnapshot = potSnapshot
		};
	}

	private void ProcessSnapshotAddReport(SnapshotAddReport snapshotAddReport)
	{
		switch (snapshotAddReport.AddStatus)
		{
			case SnapshotAddStatus.PotNotFound:
			{
				PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

				Report.GetOrCreateReport(snapshotAddReport.Key).SkipExistsCount++;
				log.WriteInfo($"Snapshot skipped - Pot unknown for key '{snapshotAddReport.Key}'. Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
				break;
			}

			case (SnapshotAddStatus.PotNotActive):
			{
				Pot pot = snapshotAddReport.Pot;
				PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

				Report.GetOrCreateReport(pot).SkipNotActiveCount++;
				log.WriteInfo($"Snapshot skipped - Pot '{pot.Name}' ({pot.Id:D}) is not active for date {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
				break;
			}

			case SnapshotAddStatus.SnapshotAlreadyExists:
			{
				Pot pot = snapshotAddReport.Pot;
				PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

				Report.GetOrCreateReport(pot).SkipExistsCount++;
				log.WriteInfo($"Snapshot skipped - Pot '{pot.Name}' ({pot.Id:D}); Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
				break;
			}

			case SnapshotAddStatus.Success:
			{
				Pot pot = snapshotAddReport.Pot;
				PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

				Report.GetOrCreateReport(pot).AddCount++;
				log.WriteInfo($"Snapshot added - Pot '{pot.Name}' ({pot.Id:D}); Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");

				break;
			}

			default:
				throw new ArgumentOutOfRangeException("Invalid status reported when adding Snapshot to Pot.", nameof(snapshotAddReport));
		}
	}

	private void LogReports()
	{
		log.WriteInfo("---> Import finished.");

		foreach (PotImportReport potImportReport in Report)
		{
			string potName = potImportReport.PotName;
			Guid potId = potImportReport.PotId;
			int addCount = potImportReport.AddCount;
			int skipCount = potImportReport.SkipExistsCount;

			log.WriteInfo($"Imported snapshots into {potName} ({potId:D}). Added: {addCount}; Skipped: {skipCount}");
		}
	}
}