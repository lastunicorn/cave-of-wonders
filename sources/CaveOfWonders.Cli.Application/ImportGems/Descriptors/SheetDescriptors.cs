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

using DustInTheWind.CaveOfWonders.Ports.SheetsAccess;

namespace DustInTheWind.CaveOfWonders.Cli.Application.ImportGems.Descriptors;

internal class SheetDescriptors
{
    private readonly Dictionary<PotCategory, ISheetDescriptor> descriptors = new()
    {
        { PotCategory.Bcr, new BcrSheetDescriptor() },
        { PotCategory.Ing, new IngSheetDescriptor() },
        { PotCategory.Brd, new BrdSheetDescriptor() },
        { PotCategory.Bt, new BtSheetDescriptor() },
        { PotCategory.Revolut, new RevolutSheetDescriptor() },
        { PotCategory.Cash, new CashSheetDescriptor() },
        { PotCategory.Gold, new GoldSheetDescriptor() }
    };

    public ISheetDescriptor Get(PotCategory potCategory)
    {
        return descriptors[potCategory];
    }
}