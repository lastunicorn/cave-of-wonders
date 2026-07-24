using DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots;

internal class ImportPotSnapshotsUseCase : IRequestHandler<ImportPotSnapshotsRequest, ImportPotSnapshotsResponse>
{
	private readonly IUnitOfWork unitOfWork;
	private readonly ISheets sheets;
	private readonly ILog log;

	public ImportPotSnapshotsUseCase(IUnitOfWork unitOfWork, ISheets sheets, ILog log)
	{
		this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		this.sheets = sheets ?? throw new ArgumentNullException(nameof(sheets));
		this.log = log ?? throw new ArgumentNullException(nameof(log));
	}

	public Task<ImportPotSnapshotsResponse> Handle(ImportPotSnapshotsRequest request, CancellationToken cancellationToken)
	{
		if (request.SourceFilePath is null)
			throw new SourceFileNotProvidedException();

		if (request.MappingsFilePath is null)
			throw new SheetMappingsNotProvidedException();

		string sourceFilePath = request.SourceFilePath;
		string importType = request.Overwrite
			? "overwrite"
			: "append";

		return log.ExecuteInfo(
			$"Starting import from file: {sourceFilePath}; Import type: {importType};",
			async () =>
			{
				List<SheetMapping> sheetMappings = sheets.GetMappings(request.MappingsFilePath)
					.ToList();

				if (request.Overwrite)
					ClearPots(sheetMappings);

				SnapshotImportReport importReport = await DoImport(request.SourceFilePath, sheetMappings);

				await unitOfWork.SaveChangesAsync(cancellationToken);

				return new ImportPotSnapshotsResponse
				{
					Report = importReport.ToList()
				};
			});
	}

	private void ClearPots(List<SheetMapping> sheetMappings)
	{
		IEnumerable<Guid> potIds = sheetMappings
			.SelectMany(x => x.ColumnDescriptors.Select(z => z.Key));

		foreach (Guid potId in potIds)
			unitOfWork.PotSnapshotRepository.RemoveByPotId(potId);
	}

	private async Task<SnapshotImportReport> DoImport(string sourceFilePath, List<SheetMapping> sheetDescriptors)
	{
		using IExcelSpreadsheet excelSpreadsheet = sheets.GetExcelSpreadsheet(sourceFilePath);
		IEnumerable<SheetValue> sheetsValues = excelSpreadsheet.Read(sheetDescriptors);

		SnapshotImport snapshotImport = new(log, unitOfWork);
		await snapshotImport.Execute(sheetsValues);

		return snapshotImport.Report;
	}
}