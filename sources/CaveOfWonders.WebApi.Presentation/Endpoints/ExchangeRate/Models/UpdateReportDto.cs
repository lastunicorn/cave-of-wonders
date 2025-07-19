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

namespace CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate.Models;

/// <summary>
/// Details about an updated exchange rate record
/// </summary>
public class UpdateReportDto
{
    /// <summary>
    /// Date of the exchange rate
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Currency pair identifier
    /// </summary>
    public string CurrencyPair { get; set; } = string.Empty;

    /// <summary>
    /// Original value before update
    /// </summary>
    public decimal OldValue { get; set; }

    /// <summary>
    /// New value after update
    /// </summary>
    public decimal NewValue { get; set; }
}