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

using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.GemStorage;

internal static class JGemExtensions
{
    public static Gem ToGem(this JGem jGem)
    {
        Gem gem = new()
        {
            Id = jGem.Id,
            ExternalId = jGem.ExternalId,
            Date = jGem.Date,
            Amount = jGem.Amount,
            Description = jGem.Description
        };

        if (Enum.TryParse(jGem.Category, out GemCategory category))
            gem.Category = category;

        foreach (KeyValuePair<string, string> jGemParameter in jGem.Parameters)
            gem.Parameters.Add(jGemParameter.Key, jGemParameter.Value);
                
        return gem;
    }
}