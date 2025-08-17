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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Pot.Models;

/// <summary>
/// Information about a pot instance at a specific point in time
/// </summary>
public class PotInstanceInfoDto
{
    /// <summary>
    /// Unique identifier of the pot
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the pot
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current value of the pot in its original currency
    /// </summary>
    public CurrencyValueDto Value { get; set; }

    /// <summary>
    /// Normalized value of the pot (converted to a common currency)
    /// </summary>
    public CurrencyValueDto NormalizedValue { get; set; }

    /// <summary>
    /// Indicates whether the pot is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Creates a DTO from the application layer PotInstanceInfo
    /// </summary>
    public static PotInstanceInfoDto From(PotInstanceInfo potInstanceInfo)
    {
        if (potInstanceInfo == null)
            return null;

        return new PotInstanceInfoDto
        {
            Id = potInstanceInfo.Id,
            Name = potInstanceInfo.Name,
            Value = CurrencyValueDto.From(potInstanceInfo.Value),
            NormalizedValue = CurrencyValueDto.From(potInstanceInfo.NormalizedValue),
            IsActive = potInstanceInfo.IsActive
        };
    }
}