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

using DustInTheWind.CaveOfWonders.Cli.Application.Convert;

namespace CaveOfWonders.WebApi.Presentation.Models;

/// <summary>
/// Response model for currency conversion
/// </summary>
public class ExchangeRateConvertResponseDto
{
    /// <summary>
    /// Original value before conversion
    /// </summary>
    public decimal InitialValue { get; set; }
    
    /// <summary>
    /// Value after conversion
    /// </summary>
    public decimal ConvertedValue { get; set; }
    
    /// <summary>
    /// Indicates whether the exchange rate is current for the requested date
    /// </summary>
    public bool IsDateCurrent { get; set; }
    
    /// <summary>
    /// Exchange rate information
    /// </summary>
    public ExchangeRateInfoDto ExchangeRate { get; set; } = new();
    
    /// <summary>
    /// Creates a response DTO from the application response
    /// </summary>
    public static ExchangeRateConvertResponseDto FromApplication(ConvertResponse response)
    {
        return new ExchangeRateConvertResponseDto
        {
            InitialValue = response.InitialValue,
            ConvertedValue = response.ConvertedValue,
            IsDateCurrent = response.IsDateCurrent,
            ExchangeRate = new ExchangeRateInfoDto
            {
                SourceCurrency = response.ExchangeRate.SourceCurrency,
                DestinationCurrency = response.ExchangeRate.DestinationCurrency,
                Date = response.ExchangeRate.Date,
                Value = response.ExchangeRate.Value
            }
        };
    }
}
