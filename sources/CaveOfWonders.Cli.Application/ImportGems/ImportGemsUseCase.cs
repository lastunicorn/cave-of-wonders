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
        log.WriteInfo(new string('-', 100));
        log.WriteInfo($"---> Starting import of {request.PotCategory.ToDisplayString()} Sheet.");
        log.WriteInfo($"---> Import from file: {request.SourceFilePath}");

        string importTypeDescription = request.Overwrite
            ? "overwrite"
            : "append";

        log.WriteInfo($"---> Import type: {importTypeDescription}");

        GemImport gemImport = new()
        {
            Pots = await RetrievePotsToPopulate(request.PotCategory),
            Overwrite = request.Overwrite,
            Log = log
        };

        IEnumerable<SheetValue> sheetsValues = GetRecordsToImport(request);
        gemImport.Execute(sheetsValues);

        await unitOfWork.SaveChanges();

        log.WriteInfo(new string('-', 100));

        return new ImportGemsResponse
        {
            Report = gemImport.Report.ToList()
        };
    }

    private Task<PotCollection> RetrievePotsToPopulate(PotCategory potCategory)
    {
        return potCategory switch
        {
            PotCategory.Bcr => RetrieveBcrPotsToPopulate(),
            PotCategory.Ing => RetrieveIngPotsToPopulate(),
            PotCategory.Brd => RetrieveBrdPotsToPopulate(),
            PotCategory.Bt => RetrieveBtPotsToPopulate(),
            PotCategory.Revolut => RetrieveRevolutPotsToPopulate(),
            PotCategory.Cash => RetrieveCashPotsToPopulate(),
            PotCategory.Gold => RetrieveGoldPotsToPopulate(),
            _ => throw new ArgumentOutOfRangeException(nameof(potCategory), potCategory, null)
        };
    }

    private async Task<PotCollection> RetrieveBcrPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Bcr.CurrentAccountLei);
        pots.Add(currentAccountLeiPot, "current-account");

        Pot savingsAccountLeiPot = await GetPot(PotIds.Bcr.SavingsAccountLei);
        pots.Add(savingsAccountLeiPot, "savings-account");

        Pot depositAccountLeiPot = await GetPot(PotIds.Bcr.DepositAccountLei);
        pots.Add(depositAccountLeiPot, "deposit-account");

        Pot currentAccountEuroPot = await GetPot(PotIds.Bcr.CurrentAccountEuro);
        pots.Add(currentAccountEuroPot, "current-account-euro");

        return pots;
    }

    private async Task<PotCollection> RetrieveIngPotsToPopulate()
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

    private async Task<PotCollection> RetrieveBrdPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Brd.CurrentAccountLei);
        pots.Add(currentAccountLeiPot, "current-account");

        return pots;
    }

    private async Task<PotCollection> RetrieveBtPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Bt.CurrentAccountEuro);
        pots.Add(currentAccountLeiPot, "current-account-euro");

        return pots;
    }

    private async Task<PotCollection> RetrieveRevolutPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Revolut.CurrentAccountLei);
        pots.Add(currentAccountLeiPot, "current-account");

        Pot currentAccountEuroPot = await GetPot(PotIds.Revolut.CurrentAccountEuro);
        pots.Add(currentAccountEuroPot, "current-account-euro");

        Pot savingsAccountLeiPot = await GetPot(PotIds.Revolut.SavingsAccountLei);
        pots.Add(savingsAccountLeiPot, "savings-account");

        return pots;
    }

    private async Task<PotCollection> RetrieveCashPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Cash.Lei);
        pots.Add(currentAccountLeiPot, "lei");

        Pot currentAccountEuroPot = await GetPot(PotIds.Cash.Euro);
        pots.Add(currentAccountEuroPot, "euro");

        return pots;
    }

    private async Task<PotCollection> RetrieveGoldPotsToPopulate()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Gold.Bcr);
        pots.Add(currentAccountLeiPot, "bcr");

        return pots;
    }

    private async Task<Pot> GetPot(string potId)
    {
        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetByPartialId(potId);
        return pots.Single();
    }

    private IEnumerable<SheetValue> GetRecordsToImport(ImportGemsRequest request)
    {
        return request.PotCategory switch
        {
            PotCategory.Bcr => sheets.GetBcrRecords(request.SourceFilePath),
            PotCategory.Ing => sheets.GetIngRecords(request.SourceFilePath),
            PotCategory.Brd => sheets.GetBrdRecords(request.SourceFilePath),
            PotCategory.Bt => sheets.GetBtRecords(request.SourceFilePath),
            PotCategory.Revolut => sheets.GetRevolutRecords(request.SourceFilePath),
            PotCategory.Cash => sheets.GetCashRecords(request.SourceFilePath),
            PotCategory.Gold => sheets.GetGoldRecords(request.SourceFilePath),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}