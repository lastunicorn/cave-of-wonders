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

    public DateTime StartDate { get; }

    public DateTime? EndDate { get; }

    public string Currency { get; }

    public int GemCount { get; }

    public DateTime? LastGemDate { get; set; }

    public CurrencyValue Value { get; set; }
    
    public List<string> Labels { get; }

    internal PotDetails(Pot pot)
    {
        Id = pot.Id;
        Name = pot.Name;
        Description = pot.Description;
        StartDate = pot.StartDate;
        EndDate = pot.EndDate;
        Currency = pot.Currency;
        GemCount = pot.Gems.Count;
        Labels = pot.Labels?.ToList() ?? [];

        Gem lastGem = pot.Gems?.Count > 0
            ? pot.Gems[^1]
            : null;

        if (lastGem != null)
        {
            LastGemDate = lastGem.Date;
            Value = new CurrencyValue
            {
                Currency = pot.Currency,
                Value = lastGem.Value
            };
        }
    }
}