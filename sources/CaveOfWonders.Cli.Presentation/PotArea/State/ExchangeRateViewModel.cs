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

using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.State;

public class ExchangeRateViewModel
{
    public string SourceCurrency { get; }

    public string DestinationCurrency { get; }

    public decimal Value { get; }

    public DateTime CurrencyDate { get; }
    
    public bool IsCurrent { get; }

    public ExchangeRateViewModel(ExchangeRateInfo exchangeRateInfo, DateTime currentDate)
    {
        SourceCurrency = exchangeRateInfo.SourceCurrency;
        DestinationCurrency = exchangeRateInfo.DestinationCurrency;
        Value = exchangeRateInfo.Value;
        CurrencyDate = exchangeRateInfo.Date;
        IsCurrent = exchangeRateInfo.Date == currentDate;
    }

    public override string ToString()
    {
        return $"1 {SourceCurrency} = {Value} {DestinationCurrency} ({CurrencyDate:d})";
    }
}