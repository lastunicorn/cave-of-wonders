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

namespace CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate.Models
{
    /// <summary>
    /// Source options for exchange rate data import
    /// </summary>
    public enum ExchangeRateImportSourceDto
    {
        /// <summary>
        /// Import from BNR website (National Bank of Romania)
        /// </summary>
        BnrWebsite = 0,

        /// <summary>
        /// Import from BNR file
        /// </summary>
        BnrFile = 1,

        /// <summary>
        /// Import from NBR file
        /// </summary>
        BnrNbrFile = 2
    }
}