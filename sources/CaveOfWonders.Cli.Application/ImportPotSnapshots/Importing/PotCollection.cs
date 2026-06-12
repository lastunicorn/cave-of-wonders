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

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportPotSnapshots.Importing;

internal class PotCollection
{
    private readonly Dictionary<Guid, Pot> pots = [];

    internal void AddRange(IEnumerable<Pot> pots)
    {
        ArgumentNullException.ThrowIfNull(pots);

        IEnumerable<Pot> potsNotNull = pots.Where(x => x != null);

        foreach (Pot pot in potsNotNull)
            this.pots.Add(pot.Id, pot);
    }

    public void ClearSnapshots()
    {
        foreach (Pot pot in pots.Values)
            pot.Snapshots.Clear();
    }

    public void ClearSnapshots(IEnumerable<Guid> potIds)
    {
        foreach (Guid potId in potIds)
        {
            bool success = pots.TryGetValue(potId, out Pot pot);

            if (success)
                pot.Snapshots.Clear();
        }
    }

    public SnapshotAddReport AddSnapshot(Guid key, PotSnapshot potSnapshot)
    {
        ArgumentNullException.ThrowIfNull(potSnapshot);

        bool potExists = pots.TryGetValue(key, out Pot pot);

        if (!potExists)
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

        if (pot.Snapshots.Contains(potSnapshot))
        {
            return new SnapshotAddReport
            {
                AddStatus = SnapshotAddStatus.SnapshotAlreadyExists,
                Key = key,
                Pot = pot,
                PotSnapshot = potSnapshot
            };
        }

        pot.Snapshots.Add(potSnapshot);

        return new SnapshotAddReport
        {
            AddStatus = SnapshotAddStatus.Success,
            Key = key,
            Pot = pot,
            PotSnapshot = potSnapshot
        };
    }
}