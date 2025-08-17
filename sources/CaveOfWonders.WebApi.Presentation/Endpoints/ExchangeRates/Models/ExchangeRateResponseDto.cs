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

namespace CaveOfWonders.WebApi.Presentation.Endpoints.ExchangeRate.Models;

/// <summary>
/// Response model containing exchange rates information
/// </summary>
public class ExchangeRateResponseDto
{
    /// <summary>
    /// Exchange rates grouped by date
    /// </summary>
    public List<DailyExchangeRatesDto> DailyExchangeRates { get; set; } = new();
    
    /// <summary>
    /// Any comments or notes about the exchange rates
    /// </summary>
    public string Comments { get; set; }
    
    /// <summary>
    /// Creates a response DTO from the application response
    /// </summary>
    public static ExchangeRateResponseDto FromApplication(PresentExchangeRateResponse response)
    {
        ExchangeRateResponseDto dto = new()
        {
            DailyExchangeRates = response.DailyExchangeRates?
                .Select(x => new DailyExchangeRatesDto
                    {
                        Date = x.Date,
                        ExchangeRates = x.ExchangeRates
                            .Select(er => new ExchangeRateForCurrencyDto
                            {
                                CurrencyPair = er.CurrencyPair,
                                Value = er.Value
                            })
                            .ToList()
                    })
                .ToList() ?? []
        };

        if (response.Comments != null)
            dto.Comments = response.Comments.ToString();

        return dto;
    }
}
