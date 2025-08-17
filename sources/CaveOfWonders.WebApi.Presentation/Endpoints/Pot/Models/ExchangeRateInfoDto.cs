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
/// Exchange rate information for currency conversion
/// </summary>
public class ExchangeRateInfoDto
{
    /// <summary>
    /// Source currency
    /// </summary>
    public string SourceCurrency { get; set; }
    
    /// <summary>
    /// Destination currency
    /// </summary>
    public string DestinationCurrency { get; set; }
    
    /// <summary>
    /// Date of the exchange rate
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Exchange rate value
    /// </summary>
    public decimal Value { get; set; }

    internal static ExchangeRateInfoDto From(ExchangeRateInfo exchangeRateInfo)
    {
        return new ExchangeRateInfoDto
        {
            SourceCurrency = exchangeRateInfo.SourceCurrency,
            DestinationCurrency = exchangeRateInfo.DestinationCurrency,
            Date = exchangeRateInfo.Date,
            Value = exchangeRateInfo.Value
        };
    }
}