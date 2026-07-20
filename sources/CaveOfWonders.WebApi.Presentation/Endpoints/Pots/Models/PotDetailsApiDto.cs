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

using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pots.Models;

public class PotDetailsApiDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Currency { get; set; }

    public int SnapshotCount { get; set; }

    public DateOnly? LastSnapshotDate { get; set; }

    public DatedAmount Value { get; set; }

    public List<string> Labels { get; set; }

    internal static PotDetailsApiDto From(PotDetails potDetails)
    {
        if (potDetails == null)
            return null;

        return new PotDetailsApiDto()
        {
            Id = potDetails.Id,
            Name = potDetails.Name,
            Description = potDetails.Description,
            StartDate = potDetails.StartDate,
            EndDate = potDetails.EndDate,
            Currency = potDetails.Currency,
            SnapshotCount = potDetails.SnapshotCount,
            Labels = potDetails.Labels?.ToList() ?? [],
            LastSnapshotDate = potDetails.LastSnapshotDate,
            Value = potDetails.Value
        };
    }
}