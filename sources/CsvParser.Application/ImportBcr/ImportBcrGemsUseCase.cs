// Cave of Wonders
// Copyright (C) 2023 Dust in the Wind
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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser.Application.ImportBcr;

public class ImportBcrGemsUseCase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISheets sheets;
    private readonly ILog log;
    private ImportReport report;

    public string SourceFilePath { get; set; }

    public bool Overwrite { get; set; }

    public ImportBcrGemsUseCase(IUnitOfWork unitOfWork, ISheets sheets, ILog log)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.sheets = sheets ?? throw new ArgumentNullException(nameof(sheets));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<ImportBcrResponse> Execute()
    {
        log.WriteInfo(new string('-', 100));
        log.WriteInfo("---> Starting import of BCR Sheet.");
        log.WriteInfo($"---> Import from file: {SourceFilePath}");

        string importTypeDescription = Overwrite
            ? "overwrite"
            : "append";

        log.WriteInfo($"---> Import type: {importTypeDescription}");

        report = new ImportReport();

        Pot currentAccountLeiPot = await GetPot(PotIds.Bcr.CurrentAccountLei);
        Pot savingsAccountLeiPot = await GetPot(PotIds.Bcr.SavingsAccountLei);
        Pot depositAccountLeiPot = await GetPot(PotIds.Bcr.DepositAccountLei);
        Pot currentAccountEuroPot = await GetPot(PotIds.Bcr.CurrentAccountEuro);

        if (Overwrite)
        {
            currentAccountLeiPot.Gems.Clear();
            savingsAccountLeiPot.Gems.Clear();
            depositAccountLeiPot.Gems.Clear();
            currentAccountEuroPot.Gems.Clear();
        }

        IEnumerable<BcrRecord> bcrRecords = sheets.GetBcrRecords(SourceFilePath);

        foreach (BcrRecord bcrRecord in bcrRecords)
        {
            if (bcrRecord.CurrentAccountLei != null)
            {
                Gem currentAccountLeiGem = new()
                {
                    Date = bcrRecord.Date,
                    Value = (double)bcrRecord.CurrentAccountLei.Value
                };

                AddGem(currentAccountLeiPot, currentAccountLeiGem);
            }

            if (bcrRecord.SavingsAccountLei != null)
            {
                Gem savingsAccountLeiGem = new()
                {
                    Date = bcrRecord.Date,
                    Value = (double)bcrRecord.SavingsAccountLei.Value
                };

                AddGem(savingsAccountLeiPot, savingsAccountLeiGem);
            }

            if (bcrRecord.DepositAccountLei != null)
            {
                Gem depositAccountLeiGem = new()
                {
                    Date = bcrRecord.Date,
                    Value = (double)bcrRecord.DepositAccountLei.Value
                };

                AddGem(depositAccountLeiPot, depositAccountLeiGem);
            }

            if (bcrRecord.CurrentAccountEuro != null)
            {
                Gem currentAccountEuroGem = new()
                {
                    Date = bcrRecord.Date,
                    Value = (double)bcrRecord.CurrentAccountEuro.Value
                };

                AddGem(currentAccountEuroPot, currentAccountEuroGem);
            }
        }

        await unitOfWork.SaveChanges();

        LogReports();
        log.WriteInfo(new string('-', 100));

        return new ImportBcrResponse
        {
            Report = report
        };
    }

    private async Task<Pot> GetPot(string potId)
    {
        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetByPartialId(potId);
        return pots.Single();
    }

    private void AddGem(Pot pot, Gem gem)
    {
        bool gemExists = pot.Gems.Contains(gem);

        if (gemExists)
        {
            report[pot].SkipCount++;
            log.WriteInfo($"Pot '{pot.Name}' ({pot.Id:D}) - Gem skipped. Date = {gem.Date:yyyy-MM-dd}; Value = {gem.Value}");

            return;
        }

        report[pot].AddCount++;
        pot.Gems.Add(gem);

        log.WriteInfo($"Pot '{pot.Name}' ({pot.Id:D}) - Gem added. Date = {gem.Date:yyyy-MM-dd}; Value = {gem.Value}");
    }

    private void LogReports()
    {
        log.WriteInfo("---> Import finished.");

        foreach (PotImportReport potImportReport in report)
        {
            string potName = potImportReport.Pot.Name;
            Guid potId = potImportReport.Pot.Id;
            int addCount = potImportReport.AddCount;
            int skipCount = potImportReport.SkipCount;

            log.WriteInfo($"Imported gems into {potName} ({potId:D}). Added: {addCount}; Skipped: {skipCount}");
        }
    }
}