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

using CsvParser.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser.Application.Importing;

internal class GemImport
{
    public PotCollection Pots { get; set; }

    public bool Overwrite { get; set; }

    public ILog Log { get; set; }

    public ImportReport Report { get; private set; }

    public void Import(IEnumerable<SheetRecord> sheetRecords)
    {
        Report = new ImportReport();

        if (Overwrite)
            Pots.ClearGems();

        foreach (SheetRecord sheetRecord in sheetRecords)
        {
            foreach (SheetValue sheetValue in sheetRecord.Values)
            {
                Gem gem = new()
                {
                    Date = sheetRecord.Date,
                    Value = (double)sheetValue.Value
                };

                GemAddReport gemAddReport = Pots.AddGem(sheetValue.Name, gem);
                ProcessGemAddReport(gemAddReport);
            }
        }

        LogReports();
    }

    private void ProcessGemAddReport(GemAddReport gemAddReport)
    {
        switch (gemAddReport.AddStatus)
        {
            case GemAddStatus.PotNotFound:
                {
                    Gem gem = gemAddReport.Gem;
                    Log.WriteInfo($"Gem skipped - Pot unknown for key '{gemAddReport.Key}'. Date = {gem.Date:yyyy-MM-dd}; Value = {gem.Value}");
                    break;
                }

            case GemAddStatus.GemAlreadyExists:
                {
                    Pot pot = gemAddReport.Pot;
                    Gem gem = gemAddReport.Gem;

                    Report[pot].SkipCount++;
                    Log.WriteInfo($"Gem skipped - Pot '{pot.Name}' ({pot.Id:D}); Date = {gem.Date:yyyy-MM-dd}; Value = {gem.Value}");
                    break;
                }

            case GemAddStatus.Success:
                {
                    Pot pot = gemAddReport.Pot;
                    Gem gem = gemAddReport.Gem;

                    Report[pot].AddCount++;
                    Log.WriteInfo($"Gem added - Pot '{pot.Name}' ({pot.Id:D}); Date = {gem.Date:yyyy-MM-dd}; Value = {gem.Value}");
                    break;
                }

            default:
                throw new ArgumentOutOfRangeException("Invalid status reported when adding Gem to Port.", nameof(gemAddReport));
        }
    }

    private void LogReports()
    {
        Log.WriteInfo("---> Import finished.");

        foreach (PotImportReport potImportReport in Report)
        {
            string potName = potImportReport.Pot.Name;
            Guid potId = potImportReport.Pot.Id;
            int addCount = potImportReport.AddCount;
            int skipCount = potImportReport.SkipCount;

            Log.WriteInfo($"Imported gems into {potName} ({potId:D}). Added: {addCount}; Skipped: {skipCount}");
        }
    }
}