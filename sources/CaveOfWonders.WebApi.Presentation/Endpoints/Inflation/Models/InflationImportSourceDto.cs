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

namespace CaveOfWonders.WebApi.Presentation.Endpoints.Inflation.Models;

/// <summary>
/// Source options for inflation data import
/// </summary>
public enum InflationImportSourceDto
{
    /// <summary>
    /// Import from a file
    /// </summary>
    File = 0,
    
    /// <summary>
    /// Import from web service
    /// </summary>
    Web = 1
}