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

using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CsvParser.Application.Importing;

internal class PotCollection
{
    private readonly Dictionary<string, Pot> pots = new();

    public void Add(Pot pot, string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (pot == null) throw new ArgumentNullException(nameof(pot));

        pots.Add(key, pot);
    }

    public void ClearGems()
    {
        foreach (Pot pot in pots.Values)
            pot.Gems.Clear();
    }

    public GemAddReport AddGem(string key, Gem gem)
    {
        if (gem == null) throw new ArgumentNullException(nameof(gem));

        bool success = pots.TryGetValue(key, out Pot pot);

        if (!success)
        {
            return new GemAddReport
            {
                AddStatus = GemAddStatus.PotNotFound,
                Key = key,
                Gem = gem
            };
        }

        if (pot.Gems.Contains(gem))
        {
            return new GemAddReport
            {
                AddStatus = GemAddStatus.GemAlreadyExists,
                Key = key,
                Pot = pot,
                Gem = gem
            };
        }

        pot.Gems.Add(gem);

        return new GemAddReport
        {
            AddStatus = GemAddStatus.Success,
            Key = key,
            Pot = pot,
            Gem = gem
        };
    }
}