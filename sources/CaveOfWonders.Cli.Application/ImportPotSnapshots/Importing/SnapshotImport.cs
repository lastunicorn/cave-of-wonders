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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Ports.SpreadsheetAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

internal class SnapshotImport
{
    public PotCollection Pots { get; set; }

    public ILog Log { get; set; }

    public SnapshotImportReport Report { get; private set; }

    public void Execute(IEnumerable<SheetValue> sheetValues)
    {
        Report = new SnapshotImportReport();

        foreach (SheetValue sheetValue in sheetValues)
        {
            PotSnapshot potSnapshot = new()
            {
                Date = sheetValue.Date,
                Value = sheetValue.Value
            };

            SnapshotAddReport snapshotAddReport = Pots.AddSnapshot(sheetValue.Key, potSnapshot);
            ProcessSnapshotAddReport(snapshotAddReport);
        }

        LogReports();
    }

    private void ProcessSnapshotAddReport(SnapshotAddReport snapshotAddReport)
    {
        switch (snapshotAddReport.AddStatus)
        {
            case SnapshotAddStatus.PotNotFound:
            {
                PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

                Report.GetOrCreateReport(snapshotAddReport.Key).SkipExistsCount++;
                Log.WriteInfo($"Snapshot skipped - Pot unknown for key '{snapshotAddReport.Key}'. Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
                break;
            }

            case (SnapshotAddStatus.PotNotActive):
            {
                Pot pot = snapshotAddReport.Pot;
                PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

                Report.GetOrCreateReport(pot).SkipNotActiveCount++;
                Log.WriteInfo($"Snapshot skipped - Pot '{pot.Name}' ({pot.Id:D}) is not active for date {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
                break;
            }

            case SnapshotAddStatus.SnapshotAlreadyExists:
            {
                Pot pot = snapshotAddReport.Pot;
                PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

                Report.GetOrCreateReport(pot).SkipExistsCount++;
                Log.WriteInfo($"Snapshot skipped - Pot '{pot.Name}' ({pot.Id:D}); Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");
                break;
            }

            case SnapshotAddStatus.Success:
            {
                Pot pot = snapshotAddReport.Pot;
                PotSnapshot potSnapshot = snapshotAddReport.PotSnapshot;

                Report.GetOrCreateReport(pot).AddCount++;
                Log.WriteInfo($"Snapshot added - Pot '{pot.Name}' ({pot.Id:D}); Date = {potSnapshot.Date:yyyy-MM-dd}; Value = {potSnapshot.Value}");

                break;
            }

            default:
                throw new ArgumentOutOfRangeException("Invalid status reported when adding Snapshot to Pot.", nameof(snapshotAddReport));
        }
    }

    private void LogReports()
    {
        Log.WriteInfo("---> Import finished.");

        foreach (PotImportReport potImportReport in Report)
        {
            string potName = potImportReport.PotName;
            Guid potId = potImportReport.PotId;
            int addCount = potImportReport.AddCount;
            int skipCount = potImportReport.SkipExistsCount;

            Log.WriteInfo($"Imported snapshots into {potName} ({potId:D}). Added: {addCount}; Skipped: {skipCount}");
        }
    }
}