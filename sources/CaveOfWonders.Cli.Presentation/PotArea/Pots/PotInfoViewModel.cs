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

using DustInTheWind.CaveOfWonders.Cli.Application;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

namespace DustInTheWind.CaveOfWonders.Cli.Presentation.PotArea.Pots;

internal class PotInfoViewModel
{
    public Guid Id { get; }

    public string Name { get; }

    public CurrencyValue Value { get; }

    public bool IsActive { get; }

    public PotInfoViewModel(PotInfo x)
    {
        if (x == null)
            return;

        Id = x.Id;
        Name = x.Name;
        Value = x.Value;
        IsActive = x.IsActive;
    }
}