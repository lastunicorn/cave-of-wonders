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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using Microsoft.AspNetCore.Mvc;

namespace CaveOfWonders.WebApi.Presentation.Models
{
    /// <summary>
    /// Request model for retrieving exchange rates
    /// </summary>
    public class ExchangeRateRequestDto
    {
        /// <summary>
        /// Start date of the date range for exchange rates
        /// </summary>
        [FromQuery]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date of the date range for exchange rates
        /// </summary>
        [FromQuery]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// First currency in the pair (3-letter code)
        /// </summary>
        [FromQuery]
        public string Currency1 { get; set; }

        /// <summary>
        /// Second currency in the pair (3-letter code)
        /// </summary>
        [FromQuery]
        public string Currency2 { get; set; }

        /// <summary>
        /// Flag to retrieve only today's exchange rates
        /// </summary>
        [FromQuery]
        public bool Today { get; set; }

        /// <summary>
        /// Specific date for which to retrieve exchange rates
        /// </summary>
        [FromQuery]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Year for which to retrieve exchange rates
        /// </summary>
        [FromQuery]
        public uint? Year { get; set; }

        /// <summary>
        /// Month for which to retrieve exchange rates (used with Year)
        /// </summary>
        [FromQuery]
        public uint? Month { get; set; }

        internal PresentExchangeRateRequest ToApplication()
        {
            PresentExchangeRateRequest request = new()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Today = Today,
                Date = Date,
                Year = Year,
                Month = Month
            };

            if (!string.IsNullOrEmpty(Currency1) && !string.IsNullOrEmpty(Currency2))
                request.CurrencyPair = $"{Currency1}/{Currency2}";

            return request;
        }
    }
}