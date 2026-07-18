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

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json.PotStorage;

internal static class PotExtensions
{
	public static JPot ToJPot(this Pot pot)
	{
		if (pot == null)
			return null;

		return new JPot
		{
			Name = pot.Name,
			Description = pot.Description,
			DisplayOrder = pot.DisplayOrder,
			StartDate = pot.StartDate,
			EndDate = pot.EndDate,
			Currency = pot.Currency,
			Labels = pot.Labels?.ToList(),
			Snapshots = pot.Snapshots
				.Select(x => x.ToJSnapshot())
				.ToList()
		};
	}
}