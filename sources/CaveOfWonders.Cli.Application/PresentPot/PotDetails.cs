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

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

public class PotDetails
{
    public Guid Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateOnly StartDate { get; }

    public DateOnly? EndDate { get; }

    public string Currency { get; }

    public int SnapshotCount { get; }

    public DateOnly? LastSnapshotDate { get; }

    public CurrencyValue Value { get; }
    
    public List<string> Labels { get; }
    
    public bool IsActive { get; }

    internal PotDetails(Pot pot)
    {
        Id = pot.Id;
        Name = pot.Name;
        Description = pot.Description;
        StartDate = pot.StartDate;
        EndDate = pot.EndDate;
        Currency = pot.Currency;
        SnapshotCount = pot.Snapshots.Count;
        Labels = pot.Labels?.ToList() ?? [];

        PotSnapshot lastPotSnapshot = pot.Snapshots?.Count > 0
            ? pot.Snapshots[^1]
            : null;

        if (lastPotSnapshot != null)
        {
            LastSnapshotDate = lastPotSnapshot.Date;
            Value = new CurrencyValue
            {
                Currency = pot.Currency,
                Value = lastPotSnapshot.Value
            };
        }
        
        IsActive = pot.EndDate == null || pot.EndDate >= DateOnly.FromDateTime(DateTime.Today);
    }
}