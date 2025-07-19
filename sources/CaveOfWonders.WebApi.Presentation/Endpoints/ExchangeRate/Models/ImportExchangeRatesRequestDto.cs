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

using System.ComponentModel.DataAnnotations;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;

namespace CaveOfWonders.WebApi.Presentation.Controllers.ExchangeRate.Models
{
    /// <summary>
    /// Request model for importing exchange rates
    /// </summary>
    public class ImportExchangeRatesRequestDto
    {
        /// <summary>
        /// Source of the exchange rates data
        /// </summary>
        [Required]
        public ExchangeRateImportSourceDto ImportSource { get; set; }

        /// <summary>
        /// Path to the source file (required when ImportSource is BnrFile or BnrNbrFile)
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Year for which to import data (optional, defaults to current year when using BnrWebsite)
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Converts the DTO to the application request model
        /// </summary>
        public ImportExchangeRatesRequest ToApplicationRequest()
        {
            return new ImportExchangeRatesRequest
            {
                ImportSource = MapImportSource(ImportSource),
                SourceFilePath = SourceFilePath,
                Year = Year
            };
        }

        private static ImportSource MapImportSource(ExchangeRateImportSourceDto source)
        {
            return source switch
            {
                ExchangeRateImportSourceDto.BnrWebsite => DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates.ImportSource.BnrWebsite,
                ExchangeRateImportSourceDto.BnrFile => DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates.ImportSource.BnrFile,
                ExchangeRateImportSourceDto.BnrNbrFile => DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates.ImportSource.BnrNbrFile,
                _ => throw new ArgumentOutOfRangeException(nameof(source), $"Unsupported import source: {source}")
            };
        }
    }
}