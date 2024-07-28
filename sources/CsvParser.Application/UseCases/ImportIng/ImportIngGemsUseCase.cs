// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
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

using CsvParser.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CsvParser.Application.Importing;
using DustInTheWind.CsvParser.Ports.SheetsAccess;
using MediatR;

namespace DustInTheWind.CsvParser.Application.UseCases.ImportIng;

internal class ImportIngGemsUseCase : IRequestHandler<ImportIngGemsRequest, ImportIngGemsResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISheets sheets;
    private readonly ILog log;

    public ImportIngGemsUseCase(IUnitOfWork unitOfWork, ISheets sheets, ILog log)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.sheets = sheets ?? throw new ArgumentNullException(nameof(sheets));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<ImportIngGemsResponse> Handle(ImportIngGemsRequest request, CancellationToken cancellationToken)
    {
        log.WriteInfo(new string('-', 100));
        log.WriteInfo("---> Starting import of ING Sheet.");
        log.WriteInfo($"---> Import from file: {request.SourceFilePath}");

        string importTypeDescription = request.Overwrite
            ? "overwrite"
            : "append";

        log.WriteInfo($"---> Import type: {importTypeDescription}");

        GemImport gemImport = new()
        {
            Pots = await CreatePotCollection(),
            Overwrite = request.Overwrite,
            Log = log
        };

        IEnumerable<SheetValue> sheetsValues = sheets.GetIngRecords(request.SourceFilePath);
        gemImport.Import(sheetsValues);

        await unitOfWork.SaveChanges();

        log.WriteInfo(new string('-', 100));

        return new ImportIngGemsResponse
        {
            Report = gemImport.Report
        };
    }

    private async Task<PotCollection> CreatePotCollection()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Ing.CurrentAccountLei);
        pots.Add(currentAccountLeiPot, "current-account");

        Pot savingsAccountLeiPot = await GetPot(PotIds.Ing.SavingsAccountLei);
        pots.Add(savingsAccountLeiPot, "savings-account");

        Pot depositAccountParintiLeiPot = await GetPot(PotIds.Ing.DepositAccountParintiLei);
        pots.Add(depositAccountParintiLeiPot, "deposit-account-parinti");

        Pot depositAccountLeiPot = await GetPot(PotIds.Ing.DepositAccountLei);
        pots.Add(depositAccountLeiPot, "deposit-account");

        return pots;
    }

    private async Task<Pot> GetPot(string potId)
    {
        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetByPartialId(potId);
        return pots.Single();
    }
}