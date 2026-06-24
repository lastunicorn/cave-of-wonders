// Cave of Wonders
// Copyright (C) 2023-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;
using DustInTheWind.CaveOfWonders.Domain;
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

        if(request.MappingsFilePath is null)
            throw new SheetMappingsNotProvidedException();

        string sourceFilePath = request.SourceFilePath;
        string importType = request.Overwrite
                ? "overwrite"
                : "append";

        return log.ExecuteInfo($"Starting import from file: {sourceFilePath}; Import type: {importType};", async () =>
        {
            List<SheetMapping> sheetMappings = GetSheetMappings(request.MappingsFilePath);
            PotCollection potCollection = await RetrievePotsToPopulate(cancellationToken);

            if (request.Overwrite)
                ClearPots(potCollection, sheetMappings);

            SnapshotImportReport importReport = DoImport(request.SourceFilePath, potCollection, sheetMappings);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ImportPotSnapshotsResponse
            {
                Report = importReport.ToList()
            };
        });
    }

    private static void ClearPots(PotCollection potCollection, List<SheetMapping> sheetDescriptors)
    {
        IEnumerable<Guid> potIds = sheetDescriptors
            .SelectMany(x => x.ColumnDescriptors.Select(z => z.Key));

        potCollection.ClearSnapshots(potIds);
    }

    private SnapshotImportReport DoImport(string sourceFilePath, PotCollection potCollection, List<SheetMapping> sheetDescriptors)
    {
        SnapshotImport snapshotImport = new()
        {
            Pots = potCollection,
            Log = log
        };

        using IExcelSpreadsheet excelSpreadsheet = sheets.GetExcelSpreadsheet(sourceFilePath);
        IEnumerable<SheetValue> sheetsValues = excelSpreadsheet.Read(sheetDescriptors);

        snapshotImport.Execute(sheetsValues);

        return snapshotImport.Report;
    }

    private List<SheetMapping> GetSheetMappings(string sheetMappingsPath)
    {
        return sheets.GetMappings(sheetMappingsPath)
            .ToList();
    }

    private async Task<PotCollection> RetrievePotsToPopulate(CancellationToken cancellationToken)
    {
        PotCollection potCollection = new();

        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetAllAsync(cancellationToken)
            .ToListAsync(cancellationToken);
        
        potCollection.AddRange(pots);

        return potCollection;
    }
}