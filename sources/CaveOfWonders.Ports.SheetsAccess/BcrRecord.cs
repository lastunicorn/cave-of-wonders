﻿// Cave of Wonders
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

namespace DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

public class BcrRecord
{
    public DateTime Date { get; set; }

    public decimal TotalLei { get; set; }

    public decimal? CurrentAccountLei { get; set; }

    public decimal? SavingsAccountLei { get; set; }

    public decimal? DepositAccountLei { get; set; }

    public decimal? CurrentAccountEuro { get; set; }

    public decimal? CurrentAccountEuroConvertedInLei { get; set; }
}