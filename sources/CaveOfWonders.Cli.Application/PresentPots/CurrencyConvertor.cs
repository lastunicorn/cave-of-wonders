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

using DustInTheWind.CaveOfWonders.Domain;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

internal class CurrencyConvertor
{
    private readonly ExchangeRate exchangeRate;
    private readonly bool isDirect;
    private readonly DateTime date;

    public DateTime Date => exchangeRate?.Date ?? date;

    public CurrencyConvertor(DateTime date)
    {
        this.date = date;
    }

    public CurrencyConvertor(ExchangeRate exchangeRate, bool isDirect = true)
    {
        this.exchangeRate = exchangeRate ?? throw new ArgumentNullException(nameof(exchangeRate));
        this.isDirect = isDirect;
    }

    public decimal Convert(decimal value)
    {
        if (exchangeRate == null)
            return 0;

        return isDirect
            ? exchangeRate.Convert(value)
            : exchangeRate.ConvertBack(value);
    }
}
