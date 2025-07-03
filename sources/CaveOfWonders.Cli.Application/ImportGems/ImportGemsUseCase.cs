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

using DustInTheWind.CaveOfWonders.Adapters.SheetsAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems.Descriptors;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportGems.Importing;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems;

internal class ImportGemsUseCase : IRequestHandler<ImportGemsRequest, ImportGemsResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISheets sheets;
    private readonly ILog log;

    public ImportGemsUseCase(IUnitOfWork unitOfWork, ISheets sheets, ILog log)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.sheets = sheets ?? throw new ArgumentNullException(nameof(sheets));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<ImportGemsResponse> Handle(ImportGemsRequest request, CancellationToken cancellationToken)
    {
        log.WriteSeparator();
        log.WriteInfo($"---> Starting import from file: {request.SourceFilePath}");

        string importTypeDescription = request.Overwrite
            ? "overwrite"
            : "append";

        log.WriteInfo($"---> Import type: {importTypeDescription}");

        ISheetDescriptor[] sheetDescriptors = [
            new BcrSheetDescriptor(),
            new IngSheetDescriptor(),
            new BrdSheetDescriptor(),
            new BtSheetDescriptor() ,
            new RevolutSheetDescriptor() ,
            new CashSheetDescriptor() ,
            new GoldSheetDescriptor() ,
            new XtbSheetDescriptor()
        ];

        PotCollection potCollection = await RetrievePotsToPopulate();

        if (request.Overwrite)
        {
            IEnumerable<Guid> potIds = sheetDescriptors
                .SelectMany(x => x.ColumnDescriptors.Select(x => x.Key));

            potCollection.ClearGems(potIds);
        }

        GemImport gemImport = new()
        {
            Pots = potCollection,
            Log = log
        };

        using IExcelSpreadsheet excelSpreadsheet = sheets.GetExcelSpreadsheet(request.SourceFilePath);
        IEnumerable<SheetValue> sheetsValues = excelSpreadsheet.Read(sheetDescriptors);

        gemImport.Execute(sheetsValues);

        await unitOfWork.SaveChanges();

        log.WriteSeparator();

        return new ImportGemsResponse
        {
            Report = gemImport.Report.ToList()
        };
    }

    private async Task<PotCollection> RetrievePotsToPopulate()
    {
        PotCollection potCollection = new();

        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetAll();
        potCollection.AddRange(pots);

        return potCollection;
    }
}